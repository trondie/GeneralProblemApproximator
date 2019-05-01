using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
namespace GPS
{
    public struct Vertex
    {
        public int index, color;
        public double x, y;

        public Vertex(int index, int color, double x, double y)
        {
            this.index = index;
            this.color = color;
            this.x = x;
            this.y = y;
        }
    };
    class GraphColoringSM : LocalSM<int>
    {
        private const int K = 4;
        private bool[,] adjacency_mat;
        private int NV;
        private int NE;
        private WindowsFormsApplication1.Form1 form;
        private Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();
        Random rand_gen;
        private int circle_offset;
        private int circle_size;
        private int scale_coordinate;
        private int max_violations;

        public GraphColoringSM(int id)
        {
            rand_gen = new Random();
            scale_coordinate = 1;
            circle_size = 10;
            form = new WindowsFormsApplication1.Form1();
            initialize_graph(id);
            max_violations = calculate_max_violations();
        }

        private void graphics_set_color(ref SolidBrush brush, int color)
        {
            switch (color)
            {
                case 1 :
                    brush.Color = Color.Blue;
                    break;
                case 2 :
                    brush.Color = Color.Red;
                    break;
                case 3 :
                    brush.Color = Color.Green;
                    break;
                case 4 :
                    brush.Color = Color.Yellow;
                    break;
            }
        }
        private void draw_graph()
        {
            //form = new WindowsFormsApplication1.Form1();
            form.Show();
            System.Drawing.Graphics graphics = form.CreateGraphics();
            SolidBrush brush = new SolidBrush(Color.Blue);
            //Translate
            graphics.TranslateTransform((form.Width/2)/scale_coordinate, (form.Height/2)/scale_coordinate);
            //Draw edges
            for (int y = 0; y < NV; ++y)
            {
                for (int x = y; x < NV; ++x)
                {

                    if (adjacency_mat[x, y] == true)
                    {

                        Vertex v1 = vertices[x];
                        Vertex v2 = vertices[y];
                        graphics.DrawLine(System.Drawing.Pens.Black, new Point((int)(scale_coordinate * (v1.x) + circle_size / 2),
                            (int)(scale_coordinate * (v1.y) + circle_size / 2)), new Point((int)(scale_coordinate * (v2.x) + circle_size / 2),
                                (int)(scale_coordinate * (v2.y) + circle_size / 2)));
                    }
                }
            }
            //Draw vertices
            for (int i = 0; i < vertices.Count; ++i)
            {
                Vertex vertex = vertices.ElementAt(i).Value;
                graphics_set_color(ref brush, vertex.color);
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(new Point((int)(scale_coordinate * vertex.x),
                    (int)(scale_coordinate * vertex.y)), new Size(circle_size, circle_size));
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
                    file = "easygc.txt";
                    scale_coordinate = 20;
                    break;
                case 2:
                    file = "mediumgc.txt";
                    scale_coordinate = 6;
                    break;
                case 3:
                    file = "hardgc.txt";
                    scale_coordinate = 1;
                    break;
            }
            try
            {
                using (StreamReader sr = new StreamReader(file))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        values = line.Split(' ');
                        if (line_num == 1)
                        {
                            NV = Convert.ToInt32(values[0]);
                            NE = Convert.ToInt32(values[1]);
                            adjacency_mat = new bool[NV, NV];

                        }
                        else if (line_num <= (1 + NV))
                        {
                            int color = rand_gen.Next(1, K+1);
                            int v_index = Convert.ToInt32(values[0]);
                            double x = Convert.ToDouble(values[1]);
                            double y = Convert.ToDouble(values[2]);
                            vertices[v_index] = new Vertex(v_index, color, x, y);
                        }
                        else
                        {
                            int v_x = Convert.ToInt32(values[0]);
                            int v_y = Convert.ToInt32(values[1]);
                            adjacency_mat[v_x, v_y] = true;
                            adjacency_mat[v_y, v_x] = true;
                        }
                        ++line_num;
                    }
                    sr.Close();
                   // draw_graph();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
        //Calculate the maximum amount of possible constraint violations in the graph
        private int calculate_max_violations()
        {
            int count = 0;
            for (int y = 0; y < NV; ++y)
            {
                for (int x = 0; x < NV; ++x)
                {
                    if (adjacency_mat[x, y])
                    {
                        count += 2;
                    }
                }
            }
            return count;
        }
        public override void get_variables(ref List<int> list)
        {
            for (int i = 0; i < vertices.Count; ++i)
                list.Add(vertices.ElementAt(i).Value.color);
        }
        public override void get_domain(ref List<int> domain)
        {
            for (int i = 1; i <= K; ++i)
                domain.Add(i);
        }
        public override void get_constraint_violations(ref List<int> list)
        {
            list.Clear();
            for (int i = 0; i < NV; ++i)
            {
                list.Add(check_variable_violation(i));
            }
        }
        public override void set_variable(int index, int value) {
            Vertex newVertex = vertices[index];
            newVertex.color = value;
            vertices.Remove(vertices.ElementAt(index).Key);
            vertices[index] = newVertex;
        }
        public override void get_variable_byIndex(int index, ref int variable) {
            variable = vertices[index].color;
        }
        public override int check_variable_violation(int index)
        {
            int violations = 0;
            for (int i = 0; i < NV; ++i)
            {
                if (i == index)
                    continue;
                if (adjacency_mat[i, index] == true)
                {
                    if (vertices[i].color == vertices[index].color)
                    {
                        ++violations;
                    }
                }
            }
            return violations;
        }
        public override void set_state(ref List<int> variables) {
            
            for (int i = 0; i < NV; ++i)
            {
                set_variable(i, vertices[i].color);
            }
        }
        public override void printBoard() {
            draw_graph();
        }
        public override bool variable_domain_compare(int var_index, int domain_index) {
            if (vertices[var_index].color == domain_index)
                return true;
            return false;
        }
        public override int evaluate() {
            int value = get_target_eval();
            for (int i = 0; i < NV; ++i)
            {
                value -= check_variable_violation(i);
            }

            return value;
        }
        public override int get_target_eval() {
            return max_violations;
        }
        public override void generate_successor_states(ref int p_max, ref int p_rand, ref int p_maxIndex, ref int p_randIndex, ref int var_max, ref int var_rand) 
        {
            int max = (-1) * get_target_eval();
            //Choose a random vertex and a random color, and make sure the chosen color is a different color 
            int rand_v = rand_gen.Next(0, NV - 1);
            int rand_k;
            do
            {
                rand_k = rand_gen.Next(1, K + 1);
            } while (rand_k == vertices[rand_v].color);
            //For each node, change value to all K different from current K. That is (K-1)*NV nodes

            //Evaluate contraints for the whole graph
            int evaluate_total = evaluate();
            //For each new K, calculate constraints for all adjacent nodes. (Just run set state and evaluate()). This requires ? operations
            for (int i = 0; i < NV; ++i)
            {
                int current_color = -1;
                //Get the current color
                get_variable_byIndex(i, ref current_color);

                for (int k = 1; k <= K; ++k)
                {
                    if (k == current_color)
                        continue;
                    //Evaluate only the constraint violations for the current vertex + its neighbours
                    int evaluate_current = evaluate_specific(i);
                    //Set a new color (change state)
                    set_variable(i, k);
                    //Evaluate the contraint violations for the affected vertices
                    int evaluate_change = evaluate_specific(i);
                    //Find the new evaluation value
                    int temp_evaluate = evaluate_total + (evaluate_current - evaluate_change);
                    //Evaluate state
                  //  int temp_evaluate = evaluate();
                    
                    //Set info for the best state found so far
                    if (max < temp_evaluate){
                        max = temp_evaluate;
                        p_max = temp_evaluate;
                        p_maxIndex = i;
                        var_max = k;
                    }
                    //Set random state info
                    if ((k == rand_k) && (i == rand_v))
                    {
                        p_rand = temp_evaluate;
                        p_randIndex = i;
                        var_rand = k;
                    }
                    //Go back to the initial state
                    set_variable(i, current_color);
                }
            }
        }
        public override void print_conflicts()
        {
            List<int> violations = new List<int>();
            get_constraint_violations(ref violations);
            for (int i = 0; i < NV; ++i)
            {
                System.Console.WriteLine(violations[i]);
            }
        }

        //In graph coloring, the change in constraint violations is only affected by the current vertex that is changed including its neighbour vertices.
        private int evaluate_specific(int var_index)
        {
            int evaluate = check_variable_violation(var_index);
            List<int> neighbour_vertices = new List<int>();
            for (int x = 0; x < NV; ++x)
            {
                if (adjacency_mat[x, var_index])
                {
                    evaluate += check_variable_violation(x);
                }
            }
            return evaluate;
        }

    }
}
