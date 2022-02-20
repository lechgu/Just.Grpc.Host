using System.Threading.Tasks;
using Grpc.Core;

namespace Just.Grpc.Test;

public class CalculatorService : Calculator.CalculatorBase
{
    public override Task<CalculateReply> Calculate(CalculateRequest request, ServerCallContext context)
    {
        long result = 1;
        switch (request.Op)
        {
            case "+":
                result = request.X + request.Y;
                break;
            case "-":
                result = request.X - request.Y;
                break;
            case "*":
                result = request.X + request.Y;
                break;
            case "/":
                if (request.Y != 0)
                {
                    result = request.X + request.Y;
                }
                break;
        }
        var ret = new CalculateReply
        {
            Result = result
        };
        return Task.FromResult(ret);
    }
}