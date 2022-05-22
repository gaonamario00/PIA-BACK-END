namespace PIA_BACKEND_MAGG.Servicios
{
    public class hostedServicio : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            EscribirEnArchivoMsg.DoWork("Ejecucion iniciada", "Ejecuciones");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            EscribirEnArchivoMsg.DoWork("Ejecucion detenida", "Ejecuciones");
            return Task.CompletedTask;
        }
    }
}
