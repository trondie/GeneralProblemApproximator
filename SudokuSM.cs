using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace GPS
{
    class SudokuSM : LocalSM<int>
    {
        private const int DIM = 9;
        private const int CONSTRAINT_SIZE = 3 * DIM;
        private int[,] board;
        private Dictionary<int, int> variables;
        private int max_eval;
        private Random rand_gen;
        private int current_eval;
        public SudokuSM(int difficulty)
        {
            board = new int[DIM, DIM];
            variables = new Dictionary<int, int>();
            //Maximum possible constraint violations (if all squares are e.g set to 1)
            //The max eval_value is used as a reference compared to the constraint violations that are subtracted from this value
            max_eval = (3 * (DIM - 1)) * (DIM * DIM);
            rand_gen = new Random();
            initialize_game(difficulty);
        }
        private void clear_board()
        {
            for (int y = 0; y < DIM; ++y)
                for (int x = 0; x < DIM; ++x)
                    board[x, y] = -1;
        }
        private void initialize_game(int difficulty)
        {
            string file = "";
            string line;
            string[] values;
            switch (difficulty)
            {
                case 1:
                    file = "easy2.txt";
                    break;
                case 2:
                    file = "mediumsudoku.txt";
                    break;
                case 3:
                    file = "diabolic.txt";
                    break;
            }
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        values = line.Select(c => c.ToString()).ToArray();
                        int tracker = 0;
                        for (int y = 0; y < DIM; ++y)
                        {
                            for (int x = 0; x < DIM; ++x)
                            {
                                board[x, y] = Convert.ToInt32(values[tracker++]);
                            }
                        }
                       // board[x, y] = Convert.ToInt32(values[x + (y * DIM)]);
                    }
                    sr.Close();
                }
                update_board();
                printBoard();

                //Fill the board with random numbers, except the numbers that are already read and set
                for (int y = 0; y < DIM; ++y)
                {
                    for (int x = 0; x < DIM; ++x)
                    {
                        if (board[x, y] == 0)
                        {
                            variables[x + (y * DIM)] = rand_gen.Next(1, DIM + 1);
                            board[x, y] = variables[x + (y * DIM)];
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
        //Returns the current "sudoku block" 
        private int get_block(int index)
        {
            return ((index % DIM) / 3) + 3 * (index / (DIM * 3));
        }
        private int get_x(int index)
        {
            int y = index / DIM;
            int x = index - (y * DIM);
            return x;
        }
        private int get_y(int index)
        {
            int y = index / DIM;
            return y;
        }
        private void update_board()
        {
            foreach (KeyValuePair<int, int> entry in variables)
            {
                board[get_x(entry.Key), get_y(entry.Key)] = entry.Value;
            }
        }

        /** Get constraint violations for a variable index **/

        private int get_row_violations(int index)
        {
            int violations = 0;
            int value = 0;
            get_variable_byIndex(index, ref value);
            int y = get_y(variables.ElementAt(index).Key);
            for (int i = 0; i < DIM; ++i)
            {
                if (board[i, y] == value)
                {
                    ++violations;
                }
            }
            return violations - 1;
        }
        private int get_column_violations(int index)
        {
            int violations = 0;
            int value = 0;
            get_variable_byIndex(index, ref value);
            int x = get_x(variables.ElementAt(index).Key);
            for (int i = 0; i < DIM; ++i)
            {
                if (board[x, i] == value)
                {
                    ++violations;
                }
            }
            return violations - 1;
        }
        private int get_square_violations(int index)
        {
            int value = 0;
            get_variable_byIndex(index, ref value);
            index = variables.ElementAt(index).Key;
            int violations = 0;
            int starty = (index / DIM) - ((index / DIM) % (int)Math.Sqrt(DIM));
            int startx = (index % DIM) - (index % (int)Math.Sqrt(DIM));

            for (int y = starty; y < starty + Math.Sqrt(DIM); ++y)
            {
                for (int x = startx; x < startx + Math.Sqrt(DIM); ++x)
                {
                    if (board[x, y] == value)
                        ++violations;
                }
            }
            return violations - 1;
        }
        private int get_index(int x, int y)
        {
            return y*DIM + x;
        }
        public override void get_variables(ref List<int> list) {
            list.Clear();
            for (int i = 0; i < variables.Count; ++i)
            {
                list.Add(variables.ElementAt(i).Value);
            }
        }
        public override void get_domain(ref List<int> domain)
        {
            domain.Clear();
            for (int i = 1; i <= DIM; ++i)
                domain.Add(i);
        }
        public override void get_constraint_violations(ref List<int> list)
        {
            list.Clear();
            update_board();
            
            for (int i = 0; i < variables.Count; ++i)
            {
                int temp = 0;    
                temp += get_row_violations(i);
                temp += get_column_violations(i);
                temp += get_square_violations(i);
                list.Add(temp);
            }
        }
        public override void set_variable(int index, int value)
        {
            int var_index = variables.ElementAt(index).Key;
            variables[var_index] = value;
            board[get_x(var_index), get_y(var_index)] = value;
        }
        public override void get_variable_byIndex(int index, ref int variable)
        {
            int var_index = variables.ElementAt(index).Key;
            variable = variables[var_index];
        }
        public override int check_variable_violation(int index)
        {
            int violations = 0;
            violations += get_column_violations(index);
            violations += get_row_violations(index);
            violations += get_square_violations(index);
            return violations;
        }
        public override void set_state(ref List<int> variables)
        {
            for (int i = 0; i < variables.Count; ++i)
            {
                this.variables[this.variables.ElementAt(i).Key] = variables[i];
            }
        }
        public override void printBoard()
        {
            update_board();
            for (int y = 0; y < DIM; ++y)
            {
                System.Console.WriteLine();
                if ( (y % 3) == 0)
                    System.Console.WriteLine("|----------------------------|");
           
                for (int x = 0; x < DIM; ++x)
                {
                    if ((x % 3) == 0)
                        System.Console.Write("|");
                    System.Console.Write(" ");
                    System.Console.Write(board[x,y]);
                    if (x == DIM-1)
                        System.Console.Write("|");
                    else
                        System.Console.Write(" ");

                }
            }
            System.Console.WriteLine();
            System.Console.WriteLine("|----------------------------|");
        }
        public override bool variable_domain_compare(int var_index, int domain_index)
        {
            if (variables[variables.ElementAt(var_index).Key] == domain_index)
                return true;
            return false;
        }
        public override int evaluate() 
        {
            int value = max_eval;
            for (int i = 0; i < variables.Count; ++i)
                value -= check_variable_violation(i);
            return value;
        }

        public override int get_target_eval()
        { 
            return max_eval;  
        }
        public override void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref int var_max, ref int var_rand) 
        {
            int max = -max_eval;
            int rand_p = rand_gen.Next(0, variables.Count);
            int rand_val = rand_gen.Next(1, DIM + 1);
            bool rand_set = false;
            int init_eval = evaluate();
            for (int index = 0; index < variables.Count; ++index)
            {
                int prev_value = 0;
                get_variable_byIndex(index, ref prev_value);
                int old_eval = check_variable_violation(index);
                //Evaluate and track the best state found so far 
                for (int i = 1; i <= DIM; ++i)
                {                
                    //Set the new state
                    set_variable(index, i);

                    int new_eval = check_variable_violation(index);
                    //Evaluate the new state
                    int temp_evaluate = init_eval - ((new_eval - old_eval) * 2);
 
                    if (max < temp_evaluate)
                    {
                        max = temp_evaluate;
                        p_max = max;
                        p_maxIndex = index;
                        var_max = i;
                    }
                    //Set the information for the randomly chosen state
                    if ((rand_p == index) && !(rand_set))
                    {
                        p_rand = temp_evaluate;
                        p_randIndex = index;
                        var_rand = rand_val;
                        rand_set = true;
                    }
                }

                //Set the current variable back to it's initial value
                set_variable(index, prev_value);


            }
        
        }
        static bool na = false;
        public override void print_conflicts() 
        {
            na = true;
            for (int y = 0; y < DIM; ++y)
            {
                System.Console.WriteLine();
                for (int x = 0; x < DIM; ++x)
                {
                    int index = x + (y * DIM);
                    System.Console.Write(" ");
                    if (variables.ContainsKey(index))
                    {
                        for (int i = 0; i < variables.Count; ++i)
                        {
                            if (variables.ElementAt(i).Key == index)
                            {
                                System.Console.Write(check_variable_violation(i));
                            }
                        }
                    }
                    else
                        System.Console.Write("X");
                    System.Console.Write(" ");
                }
            }
            System.Console.WriteLine(" ");
        }
        public virtual void show_conflicts() { }
    }
}
