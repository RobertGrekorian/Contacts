using ContactLib.Models;

namespace ContactLib.Services
{
    public interface ITestService
    {
        public string GetHelloWorld();
    }

    public class TestService : ITestService
    {
        public string GetHelloWorld()
        {
            var rey = new TestModel();
            return "Hello World 2";
        }
    }
}
