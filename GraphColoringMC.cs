using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    public class GraphColoringMC : MinConflicts<int>
    {
        public GraphColoringMC(int id)
            : base()
        {
            this.state_manager = new GraphColoringSM(id);
        }

        internal GraphColoringSM GraphColoringSM
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }
    
        public void display_results()
        {
            state_manager.set_state(ref csp.variables);
            state_manager.printBoard();
            //state_manager.show_conflicts();
        }

    }
}
