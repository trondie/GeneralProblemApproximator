using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class KQueensSA : SimAnnealing<int>
    {
        public KQueensSA(int dimension)
            : base()
        {
            this.state_manager = new KQueensSM(dimension);

            if (dimension == 25)
            {
                set_max_temp(0.02);
                set_dt(0.001);
            }
            else if (dimension == 1000)
            {
                set_max_temp(0.00003);
                set_dt(0.0000000038);
            }
            else 
            {
                set_max_temp(0.3);
                set_dt(0.0003);
            }
        }

        public KQueensSM KQueensSM
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
            state_manager.printBoard();
            state_manager.show_conflicts();
            System.Console.WriteLine(iterations);
        }
    }
}
