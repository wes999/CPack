using PowerArgs;

namespace CPack
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class CPack
    {
        public Package Package { get; set; }

        [ArgActionMethod, ArgDescription("Intializes Package")]
        public void Init()
        {
            Package = new Package();
            Console.WriteLine("Package Name:");
            Package.Name = Console.ReadLine(); 

            Console.WriteLine("Package Description:");
            Package.Description = Console.ReadLine();

            Console.WriteLine("Package Include Directory:");
            Package.IncludePath = Console.ReadLine();

            Console.WriteLine("Include Directory");

            string[] files = Directory.GetFiles(Console.ReadLine());

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith(".lib"))
                {
                    Package.LibraryFiles.Add(files[1]);
                }
            }


        }
    }

    public static class Program
    {
        public static void Main(string[] args)
        {

        }
    }
}