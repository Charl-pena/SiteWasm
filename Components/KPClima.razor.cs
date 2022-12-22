
namespace SiteWasm.Components{
    public partial class KPClima{

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
                await _ipService.SolicitarDatosPorIP(x.IP);
            
            var y = _ipService.GetCoords();
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
        /* ***RECUPERANDO DATA DEL CLIMA EN EL CACHE. SI NO AGREGANDO UNA NUEVA ENTRADA*** */
            // if (!_memoryCache.TryGetValue(SiteWasm.Data.CacheKeys.WEntry, out KPServices.OpenWeatherAPI.RootData cacheValue))
            // {
                
                await _weatherService.AsyncInicializaConCoordenadas(localLat, localLon); 
                
                var d = _weatherService.GetRootData();
                if (d is null)
                    throw new NullReferenceException("ERROR: NO SE PUEDE ASIGNAR NULL AL CACHE. REVISA LA RESPUESTA DE LA WEATHER API.");
                
                // cacheValue = d;

            //     var cacheEntryOptions = new MemoryCacheEntryOptions()
            //         .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            //     _memoryCache.Set(SiteWasm.Data.CacheKeys.WEntry, cacheValue, cacheEntryOptions);
            // }
            /* ***SE GUARDA EL NOMBRE DEL LUGAR EXTRAIDO DE LA MEMORIA CACHE*** */
            // placeName = cacheValue.name;
            placeName = d.name;
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