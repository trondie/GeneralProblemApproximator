using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class TspSA: SimAnnealing<String>
    {
        public TspSA(int problem)
            : base()
        {
            this.state_manager = new TspSM(problem);
            if (problem == 1)
            {
                set_max_temp(0.0001);
                set_dt(0.01);
            }
            else
            {
                set_max_temp(0.5);
                set_dt(0.001);//0.0001
            }
        }
        public void display_results()
        {
            state_manager.printBoard();
            System.Console.Write("Iterations : ");
            System.Console.WriteLine(iterations);
        }
        public double getDouble_eval() { return this.state_manager.getDouble_eval(); }
    }
}
