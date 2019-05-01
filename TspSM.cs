using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
namespace GPS
{
    class TspSM : LocalSM<String>
    {
        public struct Vertex
        {
            public int index;
            public double x, y;

            public Vertex(int index, double x, double y)
            {
                this.index = index;
                this.x = x;
                this.y = y;
            }
        };
        private const int NO_EDGE = -1;
        private int N;
        private int NV;
        private int NE;
        private WindowsFormsApplication1.Form1 form;
        private Dictionary<int, List<int>> adjacencies = new Dictionary<int, List<int>>();
        private Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();
        private Random rand_gen;
        private int circle_offset;
        private int circle_size;
        private int scale_coordinate;
        private int max_violations;
        private int vertexColor;
        private int start_dest;
        private double current_evaluation;

        private const bool log = false;
        int problem_id;
        int liveRefreshRate = 0;
        private List<int> path = new List<int>();
        Form liveForm;
        private bool liveDraw = false;
        System.Drawing.Rectangle rectangle2;
        System.Drawing.Graphics graphics2;
        double scaleCoord = 1;

        public TspSM(int id)
        {
          
            if (liveDraw)
            {
                liveForm = new WindowsFormsApplication1.Form1();
                liveForm.Size = new System.Drawing.Size(1900, 1500);
                liveForm.BackColor = Color.White;
                liveForm.Show();
                graphics2 = liveForm.CreateGraphics();
                graphics2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                if (id == 1)
                {
                    graphics2.TranslateTransform((liveForm.Width / 2) / 2, (liveForm.Height / 2) / (int)2);
                    graphics2.ScaleTransform(0.5f, 0.5f);
                }
                else if (id == 3)
                {
                    //graphics2.TranslateTransform((liveForm.Width / 2) / 2, (liveForm.Height / 2) / (int)2);
                    graphics2.ScaleTransform(0.5f, 0.5f);
                    scaleCoord = 1;
                    
                }
                else
                {
                    graphics2.TranslateTransform(-(liveForm.Width / 10) / 2, -(liveForm.Height / 20) / (int)2);
                    graphics2.ScaleTransform(0.4f, 0.4f);

                }
                //graphics.TranslateTransform((form.Width) / 2, (form.Height) / 2);
            }
            problem_id = id;
            rand_gen = new Random();
            scale_coordinate = 1;
            vertexColor = 1;
            circle_size = 10;
            form = new WindowsFormsApplication1.Form1();
            initialize_graph(id);
            N = vertices.Count;
            init_cycle();
            current_evaluation = evaluate_hard();
        }
        private void live_draw()
        {
            //form = new WindowsFormsApplication1.Form1();
            Form tempForm = new WindowsFormsApplication1.Form1();
            Graphics tempGraphics = tempForm.CreateGraphics();
            graphics2.Clear(Color.White);
            SolidBrush brush = new SolidBrush(Color.Blue);
            //Translate
            //    graphics.TranslateTransform((form.Width / 2) / scale_coordinate, (form.Height / 2) / scale_coordinate);
            int scaleProblem = 4;
            if (problem_id == 1)
            {
                scaleProblem = 1000;
            }

            //Draw vertices
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vertex vertex = vertices.ElementAt(i).Value;
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(new Point((int)(scale_coordinate * vertex.x * scaleCoord) / scaleProblem,
                    (int)(scale_coordinate * vertex.y * scaleCoord) / scaleProblem), new Size(circle_size, circle_size));
                graphics2.FillEllipse(brush, rectangle);
                graphics2.DrawEllipse(System.Drawing.Pens.Black, rectangle);
            }

            //Draw edges
            int edges = 0;
            for (int x = 1; x <= N; ++x)
            {
                for (int y = x; y <= N; ++y)
                {
                    if (contains_edge(x, y))
                    {
                        edges++;
                        Vertex v1 = vertices[x];
                        Vertex v2 = vertices[y];
                        graphics2.DrawLine(System.Drawing.Pens.Black, new Point((int)(scale_coordinate * (v1.x * scaleCoord) / scaleProblem + circle_size / 2),
                            (int)(scale_coordinate * (v1.y * scaleCoord) / scaleProblem + circle_size / 2)), new Point((int)(scale_coordinate * (v2.x * scaleCoord) / scaleProblem + circle_size / 2),
                                (int)(scale_coordinate * (v2.y * scaleCoord) / scaleProblem + circle_size / 2)));
                        
   
                    }
                }
            }
  
            System.Console.WriteLine(edges);
            
        }
        private bool contains_edge(int from, int to)
        {
            foreach (int v in adjacencies[from])
            {
                if (v == to)
                {
                    return true;
                }
            }
            return false;
        }
        private void draw_graph()
        {
            //form = new WindowsFormsApplication1.Form1();
            form.Show();
            System.Drawing.Graphics graphics = form.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.Blue);
            //Translate
        //    graphics.TranslateTransform((form.Width / 2) / scale_coordinate, (form.Height / 2) / scale_coordinate);
            int scaleProblem = 4;
            if (problem_id == 1)
            {
                scaleProblem = 1000;
                graphics.TranslateTransform((form.Width / 2) / 2, (form.Height / 2) / (int)2);
                graphics.ScaleTransform(0.5f, 0.5f);
            }
            if (problem_id == 3)
            {
                graphics.ScaleTransform(0.5f, 0.5f);
            }
            //Draw edges
            for (int x = 1; x <= N; ++x)
            {
                for (int y = 1; y <= N; ++y)
                {
                    if (contains_edge(x, y))
                    {
                        Vertex v1 = vertices[x];
                        Vertex v2 = vertices[y];
                        graphics.DrawLine(System.Drawing.Pens.Black, new Point((int)(scale_coordinate * (v1.x) / scaleProblem + circle_size / 2),
                            (int)(scale_coordinate * (v1.y) / scaleProblem + circle_size / 2)), new Point((int)(scale_coordinate * (v2.x) / scaleProblem + circle_size / 2),
                                (int)(scale_coordinate * (v2.y) / scaleProblem + circle_size / 2)));
                    }
                }
            }
            //Draw vertices
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vertex vertex = vertices.ElementAt(i).Value;
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(new Point((int)(scale_coordinate * vertex.x) / scaleProblem,
                    (int)(scale_coordinate * vertex.y) / scaleProblem), new Size(circle_size, circle_size));
                graphics.FillEllipse(brush, rectangle);
                graphics.DrawEllipse(System.Drawing.Pens.Black, rectangle);
            }
            
            Application.Run(form);
            
            
        }
        private void initialize_graph(int id)
        {
            string file = "";
            string line;
            int line_num = 1;
            string[] values;
            switch (id)
            {
                case 1:
                    file = "easytsp.txt";
                    scale_coordinate = 1;
                    break;
                case 2:
                    file = "mediumtsp.txt";
                    scale_coordinate = 2;
                    break;
                case 3:
                    file = "tspvlsi.txt";
                    scale_coordinate = 70;
                    break;
            }
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        values = line.Split(' ');
                        if (values[0] == "v")
                        {
                            int v_index = Convert.ToInt32(values[1]);
                     
                            double x = Convert.ToDouble(values[2]);
                            double y = Convert.ToDouble(values[3]);
                            vertices[v_index] = new Vertex(v_index, x, y);
                        }
                        else if (values[0] == "c" || values[0] == "p")
                        {
                            continue;
                        }
                        else if (values[0] == "q")
                        {
                            start_dest = Convert.ToInt32(values[1]);
                        }
                        ++line_num;
                    }
                    sr.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
        private int calculate_max_violations()
        {
            return -1;
        }
        private void add_edge(int v1, int v2){
            if (!adjacencies.ContainsKey(v1))
            {
                adjacencies.Add(v1, new List<int>());
            }
            if (!adjacencies.ContainsKey(v2))
            {
                adjacencies.Add(v2, new List<int>());
            }
            adjacencies[v1].Add(v2);
            adjacencies[v2].Add(v1);
            if (log && (adjacencies[v1].Count > 2 || adjacencies[v2].Count > 2))
            {
                System.Console.WriteLine("Error : The adjacency count is larger than two!");
                throw new Exception();
            }
        }
        private void remove_edge(int v1, int v2){
            if (adjacencies[v1].Contains(v2) && adjacencies[v2].Contains(v1))
            {
                adjacencies[v1].Remove(v2);
                adjacencies[v2].Remove(v1);
            }
            else
            {
                System.Console.WriteLine("Could not remove edge!");
                throw new Exception();
            }
        }
        private double dist(Vertex v1, Vertex v2)
        {
            return Math.Sqrt(Math.Abs(Math.Pow(v2.x - v1.x, 2.0)) + Math.Abs(Math.Pow(v2.y - v1.y, 2.0))); 
        }
        // Init a greedy cycle, O(N^2)
        private void init_cycle(){
            path.Add(start_dest);
            List<int> tempVertices = new List<int>();
            foreach (KeyValuePair<int, Vertex> v in vertices){
                tempVertices.Add(v.Key);
            }
            int current_v = start_dest;
            tempVertices.Remove(start_dest);
            while (tempVertices.Count > 0)
            {
                int minVertex = pop_min_distanceVertex(start_dest, ref tempVertices);
                if (minVertex == -1)
                {
                    Console.WriteLine("Error : The search for the minimum vertex broke!");
                }
                add_edge(current_v, minVertex);
                path.Add(minVertex);
                // Go to next vertex
                current_v = minVertex;
            }
            add_edge(current_v, start_dest);
            path.Add(start_dest);
            Console.WriteLine("Finished greedy init...");

        }
        private int pop_min_distanceVertex(int src, ref List<int> verticesList){
            double minVal = double.MaxValue;
            int vertex = -1;
            foreach (int v in verticesList) {
                double val = dist(vertices[src], vertices[v]);
                if (val < minVal)
                {
                    minVal = val;
                    vertex = v;
                }
            }
            verticesList.Remove(vertex);
            return vertex;
        }
        #region public methods
        // Dummy - Only used in MC
        public override bool variable_domain_compare(int var_index, int domain_index)
        {
            return true;
        }
        public override void printBoard()
        {
            draw_graph();
        }

        private int get_next(int previous, int from){
            foreach (int v in adjacencies[from])
            {
                if (v != previous)
                {
                    return v;
                }
            }
            return -1;
        }
        public override int evaluate() 
        {
            return Convert.ToInt32((-1) * current_evaluation);
        }
        public double evaluate_hard()
        {
            int previous = start_dest;
            int v_next = adjacencies[start_dest].ElementAt(0);
            int v_current = v_next;
            double distance = 0.0;
            int count = 0;
            do
            {
                ++count;
                distance += dist(vertices[previous], vertices[v_current]);
                v_next = get_next(previous, v_current);
                previous = v_current;
                v_current = v_next;
                if (count > N)
                {
                    System.Console.WriteLine("CYCLE IS BROKEN!");
                    throw new Exception();
                }
            } while (v_current != start_dest);
            distance += dist(vertices[previous], vertices[v_current]);
            if (count < (N - 1))
            {
                return 1.0;
            }
            if (log)
            {
                System.Console.Write("Evaluate ");
                System.Console.WriteLine(distance);
            }   
            return distance;
        }
        private void update_evaluation_by_swapping(int i, int j, int i_2, int j_2)
        {
            double change = get_change(i, j, i_2, j_2);
            current_evaluation += change;
        }
        private double get_change(int i, int j, int i2, int j2)
        {
            return dist(vertices[i], vertices[j]) + dist(vertices[i2], vertices[j2])
                 - dist(vertices[i], vertices[i2]) - dist(vertices[j], vertices[j2]);
        }
        private void swap_edges(int i, int j, int i2, int j2)
        {
            // Update evaluation
            update_evaluation_by_swapping(i, j, i2, j2);
        
            //Swap
            remove_edge(i, i2);
            remove_edge(j, j2);
            add_edge(i, j);
            add_edge(i2, j2);

           
        }
        private int cycle_next(int vertex)
        {
            int current = start_dest;
            int next = adjacencies[current].ElementAt(0);
            if (vertex == start_dest) return next;
            while (current != vertex)
            {
             //   System.Console.WriteLine(current);
                foreach (int v in adjacencies[next])
                {
                    if (v != current)
                    {
                        current = next;
                        next = v;
                        break;
                    }
                }
            }
            if (log)
            {
               // System.Console.WriteLine("NEXTING!");
            }
            return next;
        }
        // Selects two edges at random, and swaps iff there is an improvement.
        private void two_opt_swap_proposals(int iterations, ref int i_min, ref int j_min, ref int i2_min,
            ref int j2_min, ref int i_rand, ref int j_rand, ref int i2_rand, ref int j2_rand, ref int change_best_eval, ref int change_rand_eval)
        {
            double minchange = 0.0;
            int min_i = 0;
            int min_j = 0;
            int min_i2 = 0;
            int min_j2 = 0;
            bool first = true;
            double change = 0.0;
          
            for (int k = 0; k < iterations; ++k)
            {
                int i = rand_gen.Next(1, N - 1);
                int j = rand_gen.Next(i + 1, N + 1);
                int i_2 = cycle_next(i);
                int j_2 = cycle_next(j);
                change = get_change(i, j, i_2, j_2);
                if (Math.Abs(change - minchange) > 10e-6 && change < minchange)
                {
                    min_i = i;
                    min_j = j;
                    min_i2 = i_2;
                    min_j2 = j_2;
                    minchange = change;
                }
                if (first)
                {
                    i_rand = i;
                    j_rand = j;
                    i2_rand = i_2;
                    j2_rand = j_2;
                    first = false;
                    change_rand_eval = Convert.ToInt32(change);
                }
            }
            if (min_i != 0 || min_j != 0 || min_i2 != 0 || min_j2 != 0)
            {
                i_min = min_i;
                j_min = min_j;
                i2_min = min_i2;
                j2_min = min_j2;
                change_best_eval = Convert.ToInt32(minchange);
            }
        }
        public override void set_variable(int var, String vals)
        {
            List<int> val = vals.Split(',').Select(int.Parse).ToList();
            if ((val[0] == 0) && (val[1] == 0) && (val[2] == 0) && (val[3] == 0))
            {
                //Do nothing
            }
            else if ((val[0] != -1) && (val[1] != -1) && (val[2] != -1) && (val[3] != -1))
            {
                swap_edges(val[0], val[1], val[2], val[3]);
            }
            if (liveDraw)
            {
                Thread.Sleep(liveRefreshRate);
                live_draw();
            }
        }
        public override int get_target_eval()
        {
            // It's impossible to know the optimal path beforehand :) This will exhaust the SA until it reaches the max amount of iterations. 
            return 0;
        }
        public override double getDouble_eval(){
            double hard_eval = evaluate_hard();
            if (hard_eval != current_evaluation)
            {
                System.Console.Write("MISMATCH! : ");
                System.Console.Write(hard_eval);
                System.Console.Write(", ");
                System.Console.Write(current_evaluation);
            }
            return current_evaluation;
        }
        public override void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref String var_max, ref String var_rand)
        { //(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref V var_max, ref V var_rand)
            // Move this one to be set somewhere else.
            int K = N / 1 ;

            var_max = "";
            var_rand = "";
            
            int i_min = -1, j_min = -1, i2_min = -1, j2_min = -1, i_rand = -1, j_rand = -1, i2_rand = -1, j2_rand  = -1;
            int change_max = 0;
            int change_rand = 0;

            // This will generate successor states, where one is optimal among K random states, and one is random of the same K random states.
            two_opt_swap_proposals(K, ref i_min, ref j_min, ref i2_min, ref j2_min, ref i_rand, ref j_rand, ref i2_rand, ref j2_rand, ref change_max, ref change_rand);
            
            // The change in evaluation for both 
            p_max = (evaluate() + ((-1) * change_max));
            p_rand = (evaluate() + ((-1) * change_rand));
            var_max = i_min + "," + j_min + "," + i2_min + "," + j2_min;
            var_rand = i_rand + "," + j_rand + "," + i2_rand + "," + j2_rand;
            //var_max.Add(i_min); var_max.Add(j_min); var_max.Add(i2_min); var_max.Add(j2_min);
            //var_rand.Add(i_rand); var_rand.Add(j_rand); var_rand.Add(i2_rand); var_rand.Add(j2_rand);

            // These are not used, as we randomize the improvement, and not consider neighbours of a specific vertex in interest. 
            p_maxIndex = -1;
            p_randIndex = -1;

            if (log)
            {
                System.Console.Write("p_max evaluate : ");
                System.Console.WriteLine(p_max);
            }

        }
        #endregion
    }
}
