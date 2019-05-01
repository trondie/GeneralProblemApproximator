using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS
{

    class SimAnnealing<V> : ConstraintBasedLS<V>
    {
        //Default T_MAX and dt values
        private double T_MAX = 1.0;
        private double COLD = 0.0000001;
        private double dT = 0.001;

        private double temperature;
        private Random rand_gen;
        public int iterations;

        public SimAnnealing()
        {
            rand_gen = new Random();
        }
        public bool simulatedAnnealing_search(int steps)
        {
            bool test = true;
          //  state_manager.printBoard();
            int target_eval = state_manager.get_target_eval();
            int fp_max = 0;
            int fp_rand = 0;
            int fp_maxIndex = 0;
            int fp_randIndex = 0;
            iterations = 0;

            temperature = T_MAX;
            double current_eval = (double)this.state_manager.evaluate();
            for (int i = 0; i < steps; ++i)
            {
                current_eval = (double)state_manager.evaluate();
                if (current_eval >= target_eval)
                {
                    set_bestEvaluate((int)current_eval);
                    return true;
                }
                else if (current_eval > best_evaluation)
                    set_bestEvaluate((int)current_eval);
                V var_max = default(V);
                V var_rand = default(V);
                state_manager.generate_successor_states(ref fp_max,ref fp_rand, ref fp_maxIndex, ref fp_randIndex, ref var_max, ref var_rand);
                double q = (double)(((double)fp_max - current_eval) / current_eval);
                double wtf = q / temperature;
                double qp = Math.Exp(q / (double)temperature);
                double p = Math.Min(1.0, Math.Exp(q / (double)temperature));
  
                double x = rand_gen.NextDouble();

                if (i > 15000)
                {
                    int xr = 0;
                }
                if (x > p)
                {
                    System.Console.Write("E");
                    //Set new state equal to the Pmax
                    state_manager.set_variable(fp_maxIndex, var_max);
                }
                else
                {
                    System.Console.Write("X");
                    state_manager.set_variable(fp_randIndex, var_rand);
                }
                temperature = (temperature - dT) > 0.0 ? temperature - dT : COLD;
                ++iterations;

            }
            return false;
        }
        public void set_max_temp(double temperature)
        {
            this.T_MAX = temperature;
        }
        public void set_dt(double dt)
        {
            this.dT = dt;
        }
    }
}
