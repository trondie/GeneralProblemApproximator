using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
namespace GPS
{
    public class KQueensSM : LocalSM<int>
    {
        private int[] queens;
        private Random rand_gen;
        private int max_eval;
        public int[,] board;
        public int dimension;
        public int current_eval;

        //KQueensSM() { }
        public KQueensSM(int dimk)
        {
            queens = new int[dimk];
            board = new int[dimk, dimk];
            dimension = dimk;
            max_eval = dimension * (dimension-1);
            rand_gen = new Random();
            //Initialize board
            clear_board();
            int rand_column = 0;
            //Initially place all queens in a fixed row, but random column
            for (int i = 0; i < dimk; ++i)
            {
                rand_column = rand_gen.Next(0, dimk);
                placeQueen(rand_column, (i + 1));
             //   System.Console.WriteLine(rand_column);
            }
            //Use this first evaluation as a reference
            //This evaluation value is updated when a variable change. (See set_variable())
            current_eval = first_evaluate();
        }
        public override bool variable_domain_compare(int var_index, int domain_index)
        {
            if (queens[var_index] == domain_index)
                return true;
            return false;
        }
        #region public methods

        public override void printBoard()
        {
            update_board();
            for (int y = 0; y < dimension; ++y)
            {
                System.Console.WriteLine("");
                for (int x = 0; x < dimension; ++x)
                {
                    // System.Console.WriteLine(" ", board[y, x], " ");
                    System.Console.Write(" {0} ", board[y, x]);
                }
            }
            System.Console.WriteLine("");
        }
        //Moves queen_i to a column
        //This method does not update the constraint violations
        public override void set_variable(int index, int column)
        {
           // int old_var_eval = check_variable_violation(index);
            int old_var_eval = check_variable_violation(index); 
         //   queenMap[queenMap.ElementAt(index).Key] = column;
            queens[index] = column;
            int new_var_eval = check_variable_violation(index);
            this.current_eval = this.current_eval + ((old_var_eval - new_var_eval) * 2);
        }
        //This function is used together with generate_successor_states(..). 
        private void check_set_variable(int index, int column, ref int temp_evaluate, int old_var_eval)
        {
            //int old_current_eval = evaluate();
           // int old_var_eval = check_variable_violation(index);
            //queenMap[queenMap.ElementAt(index).Key] = column;
            queens[index] = column;
            int new_var_eval = check_variable_violation(index);
            temp_evaluate = temp_evaluate + ((old_var_eval - new_var_eval) * 2);

            //Now we just set the variable back into its previous position
           // queenMap[queenMap.ElementAt(index).Key] = prev_column;


        }
        //This function is used together with generate_successor_states(..) to only set back the variable in the inner loop.
        private void set_back_var(int index, int prev_column)
        {
           // queenMap[queenMap.ElementAt(index).Key] = prev_column;
            queens[index] = prev_column;
        }
        //Return the variables, which range from 1 to dim
        public override void get_variables(ref List<int> list)
        {
            for (int i = 0; i < dimension; ++i)
            {
               // list.Add(queenMap.ElementAt(i).Value);
                list.Add(queens[i]);
            }
        }
        //Return the domain, which is discrete (true) and range from 0, (dim-1) due to zero indexing
        public override void get_domain(ref List<int> domain)
        {
            for (int i = 0; i < dimension; ++i)
            {
                domain.Add(i);
            }
        }
        //Get all constraint violations for each queen, and put all counts in the same order as queen 1,..,k into list.
        public override void get_constraint_violations(ref List<int> list)
        {
            list.Clear();
            for (int i = 0; i < dimension; ++i)
            {
                list.Add(check_variable_violation(i));
            }
        }
        public override void set_state(ref List<int> variables)
        {
           // queenMap.Clear();
            for (int i = 0; i < dimension; ++i)
            {
               // queenMap[i + 1] = variables[i];
                queens[i] = variables[i];
            }
        }
        //Check for the number of constraint violations for a given queen. Pairwize checking is used here.
        public override int check_variable_violation(int index)
        {
            //The amount of violations for queen_i
            int violations = 0;
            //column(q_i)
            int qi_column = get_columnByIndex(index);
            int queen_variable = get_queenByIndex(index);
            int row_i = get_rowByIndex(index);


            for (int k = 1; k <= dimension; ++k)
         //   foreach( int k in Enumerable.Range( 1, dimension ) )
            {
                //ThreadPool.SetMaxThreads(5, 5);
                if (queen_variable != k)
                {

                    int qj_column = queens[k - 1];
                    int row_j = k - 1;
                    int abs = Math.Abs(row_j - row_i);
                    if ((qi_column == qj_column) || (qi_column == ((qj_column) + abs))
                        || (qi_column == ((qj_column) - abs)))
                    {
                        ++violations;
                    }
                  //  doneEvents[k] = new ManualResetEvent(false);
                    //Parallel.For(1,dimension, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    //    k =>
                    //        {
                    //            thread_check_constraints(qi_column, ref violations,  k, row_i);
                    ////ThreadPool.QueueUserWorkItem(state => thread_check_constraints(qi_column, ref violations,  k, row_i));
                    //        });
                }
  
            }

            return violations;
        }
        public void thread_check_constraints(int qi_column, ref int violations,  int k, int row_i)
        {
            int qj_column = queens[k - 1];
            int row_j = k - 1;
            int abs = Math.Abs(row_j - row_i);
            if ((qi_column == qj_column) || (qi_column == ((qj_column) + abs))
                || (qi_column == ((qj_column) - abs)))
            {
                ++violations;
            }
        }
        public override void show_conflicts()
        {
            List<int> violations = new List<int>();
            get_constraint_violations(ref violations);
            for (int i = 0; i < dimension; ++i)
            {
                System.Console.Write("QUEEN ");
                System.Console.Write(get_queenByIndex(i));
                System.Console.Write(" ");
                System.Console.Write(violations[i]);
                System.Console.WriteLine(" ");
            }
        }   
        //evaluate() just return the current evaluation that is updated in set_variable(..) and first_evaluate()
        public override int evaluate()
        {
            return current_eval;
        }
        private int first_evaluate()
        {
            int value = max_eval;
            for (int i = 0; i < dimension; ++i)
            {
                value -= check_variable_violation(i);
            }

            return value;
        }
        //Returns the maximum amount of constraint violations
        public override int get_target_eval()
        {
            return max_eval;
        }

        //public override void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref int var_max, ref int var_rand)
        //{
        //    //Max constraint violations
        //    int max = -1 * (dimension * (dimension - 1));
        //    int rand_px = rand_gen.Next(0, dimension - 1);
        //    int rand_py = rand_gen.Next(0, dimension - 1);
        //  //  int total_evaluate = evaluate();
           
        //    for (int row = 0; row < dimension; ++row)
        //    {
        //        int mstop = rand_gen.Next(1, 100);
        //        int prev_column = get_columnByIndex(row);
        //        int old_var_eval = check_variable_violation(row);
        //        for (int col = 0; col < dimension; ++col)
        //        {  
        //            if (prev_column == col)
        //                continue;
        //           int temp_evaluate = evaluate();

        //            check_set_variable(row, col, ref temp_evaluate, old_var_eval);

        //            if (max < temp_evaluate)
        //            {
        //                max = temp_evaluate;
        //                p_max = max;
        //                //The two following variables are 1) The index for the variable that gives max P. 2) The value (column) of the variable that gives max P.
        //                p_maxIndex = row;
        //                var_max = get_columnByIndex(row);
        //            }
        //            if ((rand_px == col) && (rand_py == row))
        //            {
        //                p_rand = temp_evaluate;
        //                p_randIndex = row;
        //                var_rand = get_columnByIndex(row);
        //            }
        //            //restore state variable by changing i back to previous value
        //            set_back_var(row, prev_column);
        //        }
        //    }

        //}
        public static int N = 12*12;
        public override void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref int var_max, ref int var_rand)
        {
            //Max constraint violations
            int max = -1 * (dimension * (dimension - 1));
            int rand_px = rand_gen.Next(0, dimension - 1);
            int rand_py = rand_gen.Next(0, dimension - 1);
            int randN = rand_gen.Next(0, N);
            //  int total_evaluate = evaluate();
          
            for (int i = 0; i < N; ++i)
            {

                int temp_evaluate = evaluate();
                int randVal = rand_gen.Next(1, dimension);
                int randQueen = rand_gen.Next(1, dimension);
                int old_var_eval = check_variable_violation(randQueen-1);
                int prev_column = get_columnByIndex(randQueen - 1);
                check_set_variable(randQueen-1, randVal, ref temp_evaluate, old_var_eval);
          
                if (max < temp_evaluate)
                {
                    max = temp_evaluate;
                    p_max = max;
                    //The two following variables are 1) The index for the variable that gives max P. 2) The value (column) of the variable that gives max P.
                    p_maxIndex = randQueen-1;
                    var_max = get_columnByIndex(randQueen-1);
                }
                if (N == randN)
                {
                    p_rand = temp_evaluate;
                    p_randIndex = randQueen-1;
                    var_rand = get_columnByIndex(randQueen-1);
                }
                set_back_var(randQueen-1, prev_column);
            }
            //for (int row = 0; row < dimension; ++row)
            //{
            //    int mstop = rand_gen.Next(1, 100);
            //    int prev_column = get_columnByIndex(row);
            //    int old_var_eval = check_variable_violation(row);
            //    for (int col = 0; col < dimension; ++col)
            //    {
            //        if (prev_column == col)
            //            continue;
            //        int temp_evaluate = evaluate();

            //        check_set_variable(row, col, ref temp_evaluate, old_var_eval);

            //        if (max < temp_evaluate)
            //        {
            //            max = temp_evaluate;
            //            p_max = max;
            //            //The two following variables are 1) The index for the variable that gives max P. 2) The value (column) of the variable that gives max P.
            //            p_maxIndex = row;
            //            var_max = get_columnByIndex(row);
            //        }
            //        if ((rand_px == col) && (rand_py == row))
            //        {
            //            p_rand = temp_evaluate;
            //            p_randIndex = row;
            //            var_rand = get_columnByIndex(row);
            //        }
            //        //restore state variable by changing i back to previous value
            //        set_back_var(row, prev_column);
            //    }
            //}

        }
        public override void get_variable_byIndex(int index, ref int variable)
        {
            variable = get_columnByIndex(index);
        }
        #endregion

        #region private_fields
   
        private int get_column(int queen_i)
        {
            return queens[queen_i - 1];
        }
        private int get_columnByIndex(int index)
        {
            return queens[index];
        }
        private int get_queenByIndex(int index)
        {
            return index + 1;
        }
        private int get_rowByIndex(int index)
        {
            return index;
        }
        private void placeQueen(int rand_column, int queen_i)
        {
            queens[queen_i - 1] = rand_column;
        }
        //This function is only called when the board needs to be printed 
        private void update_board()
        {
            clear_board();

            for (int y = 0; y < dimension; ++y)
            {
                board[y, get_columnByIndex(y)] = y + 1;
            }
        }
        private void clear_board()
        {
            for (int y = 0; y < dimension; ++y)
            {
                for (int x = 0; x < dimension; ++x)
                    board[y, x] = 0;
            }
        }
        #endregion
    }
}
