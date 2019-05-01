using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public class MinConflicts<V> : ConstraintBasedLS<V>
    {
        public int iterations = 0;
        public MinConflicts() : base() {}
        private Random rand_gen;

        public bool minimum_conflicts_search(int steps)
        {
            rand_gen = new Random();
            //Get the initial state 
            get_init_state();
            //state_manager.printBoard();
            for (int i = 0; i < steps; ++i)
            {
                if (solution_validates(ref this.csp))
                {
                    set_bestEvaluate(state_manager.get_target_eval());
                    return true;
                }
                //The following is only used in relation to track the best evaluation for statistics
                int current_evaluation = state_manager.get_target_eval() - count_violations();
                if (get_bestEvaluation() < current_evaluation)
                    set_bestEvaluate(current_evaluation);
                
                //Get an index for a random conflict variable
                int variable_index = rand_conflict_variable();
                //Minimize conflicts
                V var = minimize_conflicts(variable_index);
                //Set the current variable to the value from the domain that minimize conflicts
                this.csp.variables[variable_index] = var;
                //Update the state 
                this.state_manager.set_state(ref this.csp.variables);
                this.state_manager.get_constraint_violations(ref this.csp.constraints);
                ++iterations;
                int p = rand_gen.Next(0, 100);
                //Extra jiggle
                if (p < 20)
                {
                    int rand_i = rand_gen.Next(0, this.csp.domain.Count);
                    int rand_v = rand_gen.Next(0, this.csp.domain.Count);
                    this.csp.variables[variable_index] = this.csp.domain[rand_i];
                }
                state_manager.set_variable(variable_index, var);

            }
            
            return false;
        }
        public V minimize_conflicts(int var_index)
        {
            Dictionary<V, int> possible_values = new Dictionary<V, int>();
            //int conflicts = this.csp.constraints[var_index];
            int conflicts = state_manager.check_variable_violation(var_index);
            int temp_conflicts = conflicts;
            int initial_conflicts = conflicts;
            V initial_value = this.csp.variables[var_index];
            V value = this.csp.variables[var_index];

            //Go through the whole domain for the variable
            for (int i = 0; i < this.csp.domain.Count; ++i)
            {

                //Set variable at index to the current domain value
                state_manager.set_variable(var_index, csp.domain[i]);
                //Get the conflicts the current variable has 
                temp_conflicts = state_manager.check_variable_violation(var_index);
                //Add the conflicts count to the list of possible values
                possible_values[csp.domain[i]] = temp_conflicts;

                if (temp_conflicts < conflicts)
                {
                    conflicts = temp_conflicts;
                    value = this.csp.domain[i];
                }
            }
            //Set the value from the domain that gives the minimum amount of conflicts found so far
            state_manager.set_variable(var_index, initial_value);
            state_manager.check_variable_violation(var_index);
            //Check if "jiggling" has to be performed
            //Find the Z attacks that produce M attacks if there is no conflict count < M
            //If many counts has the same and smallest value, choose randomly
         
            //If initial_conflicts == conflicts, there are no conflicts that are less than M
            if (initial_conflicts == conflicts)
            {
                List<int> domain_indices = new List<int>();
                for (int i = 0; i < possible_values.Count; ++i)
                {
                    //We want to change the value of the variable, so the value of this variable index is excluded from possible domain values
                    if (state_manager.variable_domain_compare(var_index, i))
                        continue;
                    //We log all values that are equal to M
                    if (initial_conflicts == possible_values[this.csp.domain[i]])
                    {
                        domain_indices.Add(i);
                    }
                }
                //If all values in the domain for the variable are larger than the initial conflict count, the variable should not change its value
                if (domain_indices.Count == 0)
                {
                    return value;
                }
                //If there are values that are equal to M, then choose randomly among the alternatives
                else
                {
                    int randomIndex = rand_gen.Next(0, (domain_indices.Count));
                    value = this.csp.domain[domain_indices[randomIndex]];
                    //value = domain_indices[randomIndex];
                   
                }
            }
            //Check if there are many min conflicts, and choose a random domain value among these conflicts 
            else
            {
                List<int> domain_indices = new List<int>();
                for (int i = 0; i < possible_values.Count; ++i)
                {
                    //If the minimum conflict count equals another conflict count, log the index of the domain
                    if (conflicts == possible_values[this.csp.domain[i]])
                    {
                        domain_indices.Add(i);
                    }
                }
                //If it turns out that there are many domain values that have the same minimal conflict count, choose randomly
                if (domain_indices.Count > 1)
                {
                    int randomIndex = rand_gen.Next(0, (domain_indices.Count));
                    value = this.csp.domain[domain_indices[randomIndex]];
                }
            }
            return value;

        }
        public bool solution_validates(ref CSP<V> current_csp)
        {
            for (int i = 0; i < csp.constraints.Count; ++i)
            {
                if (current_csp.constraints[i] > 0)
                    return false;
            }
            return true;
        }
        //We don't need to do this in MC, but because we need to track the best evaluation, this check is performed in the loop
        public int count_violations() 
        {
            int count = 0;
            for (int i = 0; i < csp.constraints.Count; ++i)
                count += csp.constraints[i];
            return count;
        }
        //Returns the index of a random conflicted variable
        public int rand_conflict_variable()
        {
            int index;
            List<int> indices = new List<int>();
            for (int i = 0; i < csp.constraints.Count; ++i)
            {
                if (variable_has_conflict(i))
                {
                    indices.Add(i);
                }
            }
            index = rand_gen.Next(0, indices.Count);
            return indices[index];
        }
        public bool variable_has_conflict(int index)
        {
            if (this.csp.constraints[index] > 0)
                return true;
            return false;
        }
    }
}
