using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GPS
{

    public enum Method {SA = 2, MC = 1};
    class Testing
    {
        //print and log decides if results are printed and/or logged
        private bool print = true;
        private bool log = false;
        private int validated = 0;
        private int runs;
        private List<int> iterations_vector;
        private List<int> evaluations_vector;
        private Random rand_gen;
        public Testing() { }
        public Testing(bool print, bool log)
        {
            this.rand_gen = new Random();
            this.print = print;
            this.log = log;
            //Used to log the results for each run
            iterations_vector = new List<int>();
            evaluations_vector = new List<int>();
        }

        public void run_Sudoku(int difficulty, int iterations, Method method)
        {
            if (method == Method.SA)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Initial Board (Zeroes means not assigned any value) : ");
                SudokuSA sudoku = new SudokuSA(difficulty);
                sudoku.state_manager.printBoard();
                System.Console.WriteLine("Starting! ('X' means explore, 'E' means Exploit) : ");
                bool validation = sudoku.simulatedAnnealing_search(iterations);
                write_result(sudoku.iterations, validation);
                System.Console.WriteLine("");
                System.Console.WriteLine("Result : ");
                if (print)
                    sudoku.state_manager.printBoard();
                iterations_vector.Add(sudoku.iterations);
                evaluations_vector.Add(sudoku.get_bestEvaluation());
            }
            else if (method == Method.MC)
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("Initial Board (Zeroes means not assigned any value) : ");
                SudokuMC sudoku = new SudokuMC(difficulty);
                sudoku.state_manager.printBoard();
                bool validation = sudoku.minimum_conflicts_search(iterations);
                write_result(sudoku.iterations, validation);
                System.Console.WriteLine("");
                System.Console.WriteLine("Result : ");
                if (print)
                    sudoku.state_manager.printBoard();
                iterations_vector.Add(sudoku.iterations);
                evaluations_vector.Add(sudoku.get_bestEvaluation());
            }
        }
        public void run_kqueens(int difficulty, int iterations, Method method)
        {
            int k = 0;
            if (difficulty == 1)
                k = 8;
            else if (difficulty == 2)
                k = 25;
            else
                k = 1000;

            if (method == Method.SA)
            {
                KQueensSA kqueens = new KQueensSA(k);
                System.Console.WriteLine("");
                System.Console.Write("Starting with K = ");
                System.Console.Write(k);
                System.Console.WriteLine("");
                bool validation = validation = kqueens.simulatedAnnealing_search(iterations);
                write_result(kqueens.iterations, validation);
                System.Console.WriteLine("");
           //     System.Console.WriteLine("Result : ");
                System.Console.WriteLine("");
                if (print)
                    kqueens.state_manager.printBoard();
                
                iterations_vector.Add(kqueens.iterations);
                evaluations_vector.Add(kqueens.get_bestEvaluation());
            }
            else if (method == Method.MC)
            {
                KQueensMC kqueens = new KQueensMC(k);
                System.Console.WriteLine("");
                System.Console.Write("Starting with K = ");
                System.Console.Write(k);
                System.Console.WriteLine("");
                bool validation = kqueens.minimum_conflicts_search(iterations);
                write_result(kqueens.iterations, validation);
                System.Console.WriteLine("");
              //  System.Console.WriteLine("Result : ");
                System.Console.WriteLine("");
                if (print)
                    kqueens.state_manager.printBoard();
                iterations_vector.Add(kqueens.iterations);
                evaluations_vector.Add(kqueens.get_bestEvaluation());
            }
        }
        public void run_graphColoring(int difficulty, int iterations, Method method)
        {

            if (method == Method.SA)
            {
                GraphColoringSA gc = new GraphColoringSA(difficulty);
                System.Console.WriteLine("");
                System.Console.WriteLine("Starting!");
                System.Console.WriteLine("");
                bool validation = gc.simulatedAnnealing_search(iterations);
                write_result(gc.iterations, validation);
                if (print)
                    gc.state_manager.printBoard();
                iterations_vector.Add(gc.iterations);
                evaluations_vector.Add(gc.get_bestEvaluation());
            }
            else if (method == Method.MC)
            {
                GraphColoringMC gc = new GraphColoringMC(difficulty);
                System.Console.WriteLine("");
                System.Console.WriteLine("Starting!");
                System.Console.WriteLine("");
                bool validation = gc.minimum_conflicts_search(iterations);
                write_result(gc.iterations, validation);
                if (print)
                    gc.state_manager.printBoard();
                iterations_vector.Add(gc.iterations);
                evaluations_vector.Add(gc.get_bestEvaluation());
            }
        }
        public void run_tsp(int difficulty, int iterations, Method method)
        {

            if (method == Method.SA)
            {
                
                TspSA tsp = new TspSA(difficulty);
                System.Console.WriteLine("");
                System.Console.WriteLine("Starting!");
                System.Console.WriteLine("");
                bool validation = tsp.simulatedAnnealing_search(iterations);
                write_result(tsp.iterations, validation);
                iterations_vector.Add(tsp.iterations);
                evaluations_vector.Add(tsp.get_bestEvaluation());
                System.Console.Write("Current TSP distance : ");
                System.Console.WriteLine(tsp.getDouble_eval());
                if (print)
                    tsp.state_manager.printBoard();
                
            }
        }
        public void test_sudoku(int difficulty, int runs, int iterations, Method method)
        {
            this.runs = runs;
            if ((difficulty == 4))
            {
                Parallel.For(0, runs, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    k =>
                    {
                        int randSleep = rand_gen.Next(10000, 30000);
                        Thread.Sleep(randSleep);
                        run_Sudoku(difficulty, iterations, method);
                        //ThreadPool.QueueUserWorkItem(state => run_kqueens(difficulty, iterations, method));
                    });

            }
            else
            {
                for (int i = 0; i < runs; ++i)
                {
                    run_Sudoku(difficulty, iterations, method);
                    //Sleep for 1.5 seconds. (Because of random generators)
                    System.Threading.Thread.Sleep(1500);
                }
            }
            if (log)
            {
                System.Console.Write("Iterations avg/stddev : ");
                write_stat(ref iterations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Evaluations avg/stddev : ");
                write_stat(ref evaluations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Max eval : ");
                System.Console.Write(write_max_eval(ref evaluations_vector));
                System.Console.WriteLine("");
                System.Console.Write("Min iterations : ");
                System.Console.Write(min_iterations(ref iterations_vector));
            }
        }
        public void test_kqueens(int difficulty, int runs, int iterations, Method method)
        {
            this.runs = runs;
            if ((difficulty == 4) )
            {
                Parallel.For(0, runs, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    k =>
                    {
                        int randSleep = rand_gen.Next(1000, 20000);
                        Thread.Sleep(randSleep);
                        run_kqueens(difficulty, iterations, method);
                        //ThreadPool.QueueUserWorkItem(state => run_kqueens(difficulty, iterations, method));
                    });
                
            }
            else
            {
                for (int i = 0; i < runs; ++i)
                {
                    run_kqueens(difficulty, iterations, method);
                    //Sleep for 1.5 seconds. (Because of random generators)
                    System.Threading.Thread.Sleep(1500);
                }
            }
            
            if (log)
            {
                System.Console.Write("Iterations avg/stddev : ");
                write_stat(ref iterations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Evaluations avg/stddev : ");
                write_stat(ref evaluations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Max eval : ");
                System.Console.Write(write_max_eval(ref evaluations_vector));
                System.Console.WriteLine("");
                System.Console.Write("Min iterations : ");
                System.Console.Write(min_iterations(ref iterations_vector));
            }
        }
        public void test_graphColoring(int difficulty, int runs, int iterations, Method method)
        {
            this.runs = runs;
            if ((difficulty == 4))
            {
                Parallel.For(0, runs, new ParallelOptions { MaxDegreeOfParallelism = 8 },
                    k =>
                    {
                        int randSleep = rand_gen.Next(10000, 30000);
                        Thread.Sleep(randSleep);
                        run_graphColoring(difficulty, iterations, method);
                        //ThreadPool.QueueUserWorkItem(state => run_kqueens(difficulty, iterations, method));
                    });

            }
            else
            {
                for (int i = 0; i < runs; ++i)
                {
                    run_graphColoring(difficulty, iterations, method);
                    //Sleep for 1.5 seconds. (Because of random generators)
                    System.Threading.Thread.Sleep(1500);
                }
            }
            if (log)
            {
                System.Console.Write("Iterations avg/stddev : ");
                write_stat(ref iterations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Evaluations avg/stddev : ");
                write_stat(ref evaluations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Max eval : ");
                System.Console.Write(write_max_eval(ref evaluations_vector));
                System.Console.WriteLine("");
                System.Console.Write("Min iterations : ");
                System.Console.Write(min_iterations(ref iterations_vector));
            }
        }
        void test_tsp(int difficulty, int runs, int iterations, Method method)
        {
            this.runs = runs;

            for (int i = 0; i < runs; ++i)
            {
                run_tsp(difficulty, iterations, method);
                //Sleep for 1.5 seconds. (Because of random generators)
                System.Threading.Thread.Sleep(1500);
            }
            System.Console.WriteLine("Finished!");

                System.Console.Write("Iterations avg/stddev : ");
                write_stat(ref iterations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Evaluations avg/stddev : ");
                write_stat(ref evaluations_vector);
                System.Console.WriteLine("");
                System.Console.Write("Max eval : ");
                System.Console.Write(write_max_eval(ref evaluations_vector));
                System.Console.WriteLine("");
                System.Console.Write("Min iterations : ");
                System.Console.Write(min_iterations(ref iterations_vector));
        }
        
        public void setup_test(int game, int difficulty, int runs, int iterations, Method method)
        {
            if (game == 1)
            {
                test_kqueens(difficulty, runs, iterations, method);
            }
            else if (game == 2)
            {
                test_graphColoring(difficulty, runs, iterations, method);
            }
            else if (game == 3)
            {
                test_sudoku(difficulty, runs, iterations, method);
            }
            else if (game == 4)
            {
                test_tsp(difficulty, runs, iterations, method);
            }

        }
        public void check_validate(bool validate)
        {
            if (validate)
            {
                ++validated;
                System.Console.Write("Solution Validates! ");
            }
            else
            {
                System.Console.Write("Failed Validation! ");
            }
        }
        public void write_result(int iterations, bool validation)
        {
            check_validate(validation);
            System.Console.Write("Iterations : ");
            System.Console.Write(iterations);
        }
        public void intro_dialogue()
        {
            System.Console.WriteLine("Welcome to a General Problem Solver!");
            write_hline();
            System.Console.WriteLine("Choose a problem : ");
            System.Console.WriteLine("");
            System.Console.WriteLine("\t 1: K-Queens");
            System.Console.WriteLine("\t 2: Graph Coloring");
            System.Console.WriteLine("\t 3: Sudoku");
            System.Console.WriteLine("\t 4: TSP (SA only)");
            System.Console.WriteLine("");
            System.Console.Write("Choice : ");
            int puzzle_choice = int.Parse(Console.ReadLine());
            if (puzzle_choice == 1)
                System.Console.WriteLine("You chose K-Queens.");
            else if (puzzle_choice == 2)
                System.Console.WriteLine("You chose Graph Coloring");
            else if (puzzle_choice == 3)
                System.Console.WriteLine("You chose Sudoku");
            else if (puzzle_choice == 4)
                System.Console.WriteLine("You chose the Traveling Salesman Problem");
            System.Console.WriteLine("");
            System.Console.WriteLine("Choose difficulty : ");
            System.Console.WriteLine("\t 1: Easy");
            System.Console.WriteLine("\t 2: Medium");
            System.Console.WriteLine("\t 3: Hard");
            System.Console.WriteLine("");
            System.Console.Write("Choice : ");
            int difficulty_choice = int.Parse(Console.ReadLine());

            System.Console.WriteLine("Choose method :");
            System.Console.WriteLine("\t 1: Minimum Conflicts search");
            System.Console.WriteLine("\t 2: Simulated Annealing");
            System.Console.WriteLine("");
            System.Console.Write("Choice : ");
            int method_choice = int.Parse(Console.ReadLine());
            Method method;
            if (method_choice == 1){
                System.Console.WriteLine("You chose Minimum Conflicts search");
                method = Method.MC;
            }
            else{
                System.Console.WriteLine("You chose Simulated Annealing");
                method = Method.SA;
            }
            System.Console.WriteLine("");

            System.Console.WriteLine("Enter the amount of iterations :");
            System.Console.WriteLine("");
            System.Console.WriteLine("\t 1: 10k");
            System.Console.WriteLine("\t 2: 100k (Recommended for MC Sudoku)");
            System.Console.WriteLine("\t 3: Custom");
            System.Console.Write("Choice : ");

            int iteration_choice = int.Parse(Console.ReadLine());
            int iterations = 0;
            if (iteration_choice == 1)
                iterations = 10000;
            else if (iteration_choice == 2)
                iterations = 100000;
            else if (iteration_choice == 3)
            {
                System.Console.Write("Amount : ");
                int amount_choice = int.Parse(Console.ReadLine());
                iterations = amount_choice;
            }
            
            
            System.Console.WriteLine("");

            System.Console.Write("Enter the amount of times to repeat the search : ");
            int runs = int.Parse(Console.ReadLine());

            System.Console.WriteLine("");
            System.Console.WriteLine("OK!");

            setup_test(puzzle_choice, difficulty_choice, runs, iterations, method);


        }
        public void write_hline()
        {
            System.Console.WriteLine("--------------------------------------------");
        }
        private double calculate_mean(ref List<int> vec)
        {
            double mean = 0;
            for (int i = 0; i < vec.Count; ++i)
                mean += (double)vec[i];
            mean /= vec.Count;
            return mean;
        }
        private double calculate_stddev(ref List<int> vec)
        {
            double mean = calculate_mean(ref vec);
            double stddev = 0;
            for (int i = 0; i < vec.Count; ++i)
                stddev += Math.Pow(((double)vec[i] - mean), 2);
            double w = 1.0 / (double)vec.Count;
            stddev = Math.Sqrt(w * stddev);
            return stddev;
        }
        public void write_mean(ref List<int> vec)
        {
            System.Console.Write("MEAN : ");
            System.Console.Write(calculate_mean(ref vec));
            System.Console.Write(" ");
        }
        public void write_stddev(ref List<int> vec)
        {
            System.Console.Write("STDDEV : ");
            System.Console.Write(calculate_stddev(ref vec));
        }
        public void write_stat(ref List<int> vec)
        {
            write_mean(ref vec);
            write_stddev(ref vec);
            System.Console.WriteLine("");
            System.Console.Write("Validates / Failed validation : ");
            System.Console.Write(validated);
            System.Console.Write("/");
            System.Console.Write(runs-validated);
        }
        private int write_max_eval(ref List<int> vec)
        {
            int max = -1;
            for (int i = 0; i < vec.Count; ++i)
            {
                if (max < vec[i])
                    max = vec[i];
            }
            return max;
        }
        private int min_iterations(ref List<int> vec)
        {
            int min = vec[0];
            for (int i = 1; i < vec.Count; ++i)
            {
                if (min > vec[i])
                    min = vec[i];
            }
            return min;
        }
    }
}
