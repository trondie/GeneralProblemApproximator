using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class SudokuSA : SimAnnealing<int>
    {
        public SudokuSA(int problem)
            : base()
        {
            this.state_manager = new SudokuSM(problem);
            if (problem == 1)
            {
                set_max_temp(0.002);
                set_dt(0.000004);
            }
            else
            {
                set_max_temp(0.0025);
                set_dt(0.00000001);
            }


        }

        internal SudokuSM SudokuSM
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
