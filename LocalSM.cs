using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public abstract class LocalSM<V>
    {
        public virtual void get_variables(ref List<V> list) { }
        public virtual void get_domain(ref List<V> domain) { }
        public virtual void get_constraint_violations(ref List<int> list) { }
        public virtual void set_variable(int index, V value) { }
        public virtual void get_variable_byIndex(int index, ref V variable) { }
        public virtual int check_variable_violation(int index) { return -1; }
        public virtual void set_state(ref List<V> variables) { }
        public virtual void printBoard() { }
        public virtual bool variable_domain_compare(int var_index, int domain_index) { return false; }
        public virtual int evaluate() { return 1; }
        public virtual int get_target_eval() { return -1; }
        public virtual void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref V var_max, ref V var_rand) { }

        public virtual void print_conflicts() { }
        public virtual void show_conflicts() { }
        public virtual double getDouble_eval() { return -1.0; }
        public LocalSM() { }
        
    }
}
