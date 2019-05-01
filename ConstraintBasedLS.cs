using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public abstract class ConstraintBasedLS<V>
    {
        public CSP<V> csp;
        public LocalSM<V> state_manager;
        public int best_evaluation;
        public ConstraintBasedLS()
        {
            csp = new CSP<V>();
            this.csp.variables = new List<V>();
            this.csp.domain = new List<V>();
            this.csp.constraints = new List<int>();
            best_evaluation = -1;
        }

        public LocalSM<V> LocalSM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public CSP<V> CSP
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public void get_init_state()
        {
            state_manager.get_variables(ref this.csp.variables);
            state_manager.get_domain(ref this.csp.domain);
            state_manager.get_constraint_violations(ref this.csp.constraints);
        }
        public void set_bestEvaluate(int value)
        {
            best_evaluation = value;
        }
        public int get_bestEvaluation()
        {
            return best_evaluation;
        }

    }
}
