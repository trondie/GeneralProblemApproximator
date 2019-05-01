using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{
    class SudokuMC : MinConflicts<int>
    {
        public SudokuMC(int problem)
            : base()
        {
            this.state_manager = new SudokuSM(problem);
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
