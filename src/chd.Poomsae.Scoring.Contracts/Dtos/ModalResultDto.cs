using chd.Poomsae.Scoring.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chd.Poomsae.Scoring.Contracts.Dtos
{
    public class ModalResultDto<TDataType>
        where TDataType : class
    {
        public TDataType Entity { get; set; }
        public TDataType Data { get; }
        public EDataAction Action { get; set; }
        public ModalResultDto(TDataType data, EDataAction action)
        {
            this.Data = data;
            this.Action = action;
        }
    }
}
