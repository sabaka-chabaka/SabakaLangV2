namespace SabakaLang.VM;

public class Program
{
    public static void Main(string[] args)
    {
        string bytecode = File.ReadAllText(args[0]);
        VirtualMachine vm = new();
        vm.Execute(bytecode);
    }
}
