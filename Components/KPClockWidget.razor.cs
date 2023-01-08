
namespace SiteWasm.Components{
    public partial class KPClockWidget{

        private async Task TrasObtnerActualizarCoords_TimeZone(string localLat, string localLon){
            /* Las coordenadas estan guardadas correctamente en el LocalStorage */
            _geoLService.IsSuccess = true;
            _geoLService.ActualizarCoords(localLat, localLon);
            await localStore.SetItemAsync<string>("TimeZone", _ClockService.GetIANAIdByCoords(localLat, localLon));
        }
        private async Task<bool> TrasObtenerCoords_TimezoneGuardarlos(){
         await _geoLService.AsyncGetPosition();
             
         if (_geoLService.IsSuccess){
             var coords = _geoLService.GetCoords();
             await GuardarCoords_Timezone(coords.Item1, coords.Item2);
             return true;
         }
         else{
             return false;
         }
        }
        private async Task<bool> TrasObtenerIPCoord_TimezoneGuardarlos(){
            var x = await _ipWasm.GetUserIPAsync();
            Console.WriteLine(x?.IP);
            if( x?.IP is not null)
                await _ipLocationService.SolicitarDatosPorIP(x.IP);
            
            var y = _ipLocationService.GetCoords();
            if(y.HasValue)
            {
                await GuardarCoords_Timezone(y.Value.Item1, y.Value.Item2);
                return true;
            }else{
                placeName = "LocalMachine";
                theDay = DateTime.Now.ToString("MM/dd");
                return false;
            }
        }

        private async Task ObtenerDataClimaDelCacheSinoGuardarla(string localLat, string localLon){
            if(await localStore.ContainKeyAsync("PlaceName")) {
                var pN = await localStore.GetItemAsync<string>("PlaceName");
                placeName = pN;
            }
            else
            {
                try
                {
                    await _weatherService.AsyncInicializaConCoordenadas(localLat, localLon); 
                    
                }
                catch (System.Exception)
                {
                    Console.WriteLine("Error al llamar la API.");
                }
                var d = _weatherService.GetRootData();
                if (d is null){
                    Console.WriteLine("ERROR: NO SE PUEDE ASIGNAR NULL AL CACHE. REVISA LA RESPUESTA DE LA WEATHER API.");
                    placeName = "Error en la conexión...";
                }
                else
                {
                    await localStore.SetItemAsync<string>("PlaceName", d.name);
                    placeName = d.name;
                }                
            }
        }
        private async Task ObtenerTimezone_CambiarFecha(){
            /* ***SE ACTUALIZA EL TIEMPO EN BASE A LA ZONA HORARIA ENCONTRADA POR LA COORDENADAS*** */
            var _timeZone = await localStore.GetItemAsync<string>("TimeZone");
            if (_timeZone is not null)
            // theDay = _NTPClient.GetLocalTimeByID(timeZoneId).ToString("MM/dd");
            theDay = _ClockService.GetLocalTimeByID(timeZoneId).ToString("MM/dd");
        }
        private async Task GuardarCoords_Timezone(string lat, string lon){
            await localStore.SetItemAsync<string>("Lat", lat);
            await localStore.SetItemAsync<string>("Lon", lon);
            await localStore.SetItemAsync<string>("TimeZone", _ClockService.GetIANAIdByCoords(lat, lon));
        }
    }
}