using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SGB_Lobo.AutoMapper
{
    public static class MapperConfig
    {
        private static IMapper _mapper;
        private static readonly object _lock = new object();

        public static IMapper Mapper
        {
            get
            {
                if (_mapper == null)
                {
                    lock (_lock)
                    {
                        if (_mapper == null)
                        {
                            var config = new MapperConfiguration(cfg =>
                            {
                                cfg.AddProfile<AutoMapperProfile>();
                            });

                            _mapper = config.CreateMapper();
                        }
                    }
                }
                return _mapper;
            }
        }

        public static void Initialize()
        {
            // Força a inicialização do mapper
            var mapper = Mapper;
        }
    }
}