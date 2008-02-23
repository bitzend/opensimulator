/*
* Copyright (c) Tribal Media AB, http://tribalmedia.se/
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * The name of Tribal Media AB may not be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/

using System.Collections.Generic;
using TribalMedia.Framework.Data;

namespace TribalMedia.Framework.Data
{
    public class BaseSchema
    {
        protected BaseTableMapper m_tableMapper;
        protected Dictionary<string, BaseFieldMapper> m_mappings;

        public Dictionary<string, BaseFieldMapper> Fields
        {
            get { return m_mappings; }
        }

        public BaseSchema(BaseTableMapper tableMapper)
        {
            m_mappings = new Dictionary<string, BaseFieldMapper>();
            m_tableMapper = tableMapper;
        }
    }

    public class BaseSchema<TObj> : BaseSchema
    {
        public BaseSchema(BaseTableMapper tableMapper)
            : base(tableMapper)
        {
        }

        public ObjectField<TObj, TField> AddMapping<TField>(string fieldName,
                                                            ObjectGetAccessor<TObj, TField> rowMapperGetAccessor,
                                                            ObjectSetAccessor<TObj, TField> rowMapperSetAccessor)
        {
            ObjectField<TObj, TField> rowMapperField =
                new ObjectField<TObj, TField>(m_tableMapper, fieldName, rowMapperGetAccessor, rowMapperSetAccessor);

            m_mappings.Add(fieldName, rowMapperField);

            return rowMapperField;
        }
    }
}