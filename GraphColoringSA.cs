using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class GraphColoringSA : SimAnnealing<int>
    {
        public GraphColoringSA(int problem)
            : base()
        {
            this.state_manager = new GraphColoringSM(problem);
            switch (problem)
            {
                case 3:
                    set_max_temp(0.02);
                    set_dt(0.00001);
                    break;
                default:
                    set_max_temp(1.0);
                    set_dt(0.01);
                    break;
            }
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
            state_manager.printBoard();
            System.Console.Write("Iterations : ");
            System.Console.WriteLine(iterations);
        }
    }
}
