namespace GrpcGreeter {
    public partial class HelloRequest {
        public string Name => $"{FirstName} {LastName}";        
    }
}