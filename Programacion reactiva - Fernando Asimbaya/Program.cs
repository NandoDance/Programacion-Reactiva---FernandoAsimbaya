using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Programacion_reactiva___Fernando_Asimbaya
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //Obtener path local de descargas
            string userProfile = Environment.GetEnvironmentVariable("USERPROFILE");
            string downloadsPath = System.IO.Path.Combine(userProfile, "Downloads");

            // Monitorizar cambios en el directorio de descargas
            var watcher = new FileSystemWatcher(downloadsPath)
            { 
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite, 
                Filter = "*.*" 
            };

            //Operador Select: transforma los eventos en strings descrptivos
            var created = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>
                (handler => watcher.Created += handler, handler => watcher.Created -= handler)
                .Select(e => $"Archivo Creado: {e.EventArgs.Name}"); 
            
            var changed = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>
                (handler => watcher.Changed += handler, handler => watcher.Changed -= handler)
                .Select(e => $"Archivo Modificado: {e.EventArgs.Name}"); 
            
            var deleted = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>
                (handler => watcher.Deleted += handler, handler => watcher.Deleted -= handler)
                .Select(e => $"Archivo Eliminado: {e.EventArgs.Name}");

            //Operador Merge: unificar eventos creado, modificado y elminado
            var merged = created.Merge(changed).Merge(deleted);

            //Operador DistinctUntilChanged: emitir eventos que son diferentes al anterior
            var distinctEvents = merged.DistinctUntilChanged();

            //Operador Timestamp: agregar marca de tiempo a cada evento
            var timestamped = distinctEvents.Timestamp();

            //Operador Buffer: agrupar eventos en intervalos de 5 segundos
            var buffered = timestamped.Buffer(TimeSpan.FromSeconds(5));

            //Suscribirse a los streams
            buffered.Subscribe(events => { 
                Console.WriteLine("Buffer:"); 
                
                foreach (var e in events) 
                {
                    Console.WriteLine($"Timestamp: {e.Timestamp}, Evento: {e.Value}");
                } 
            }); 
                        
            watcher.EnableRaisingEvents = true;
            Console.WriteLine("-----------------------------------------------");
            Console.WriteLine("Escucha de eventos en directotrio de descarga.");
            Console.WriteLine(".....Presione cualquier tecla para salir.....");
            Console.WriteLine("-----------------------------------------------");

            Console.ReadKey();
        }
    }
}
