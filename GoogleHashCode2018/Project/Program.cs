using System;

namespace Project
{
    class Program
    {
        static void Main(string[] args)
        {
            //Manager a_example = new Manager("a_example");
            //a_example.Run();

            //Manager b_easy = new Manager("b_should_be_easy");
            //b_easy.Run();

            //Manager c_no_hurry = new Manager("c_no_hurry");
            //c_no_hurry.Run();

            Manager d_metropolis = new Manager("d_metropolis");
            d_metropolis.Run();

            Manager e_high_bonus = new Manager("e_high_bonus");
            e_high_bonus.Run();
        }
    }
}
