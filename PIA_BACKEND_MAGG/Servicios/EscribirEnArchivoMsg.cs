namespace PIA_BACKEND_MAGG.Servicios
{
    //Clase static para poder mandarla a llamar sin necesidad de instanciar
    public static class EscribirEnArchivoMsg
    {

        // Manda a llamar le metodo Escribir()
        public static void DoWork(string data, string nombreArchivo)
        {
            Escribir(data + " " + DateTime.Now.ToString("G"), nombreArchivo);
        }

        // Realiza la escritura en el archivo especificado y lo guarda en la carpeta "ArchivosTxt"
        public static void Escribir(string msg, string nombreArchivo)
        {
            string ruta = @"wwwroot/" + nombreArchivo;
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(msg);
                writer.Close();
            }
        }
    }

}
