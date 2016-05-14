﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.MetadirectoryServices;

namespace Lithnet.GoogleApps.MA
{
    internal class MASchemaType
    {
        public string Name { get; set; }

        public List<IAttributeAdapter> Attributes { get; set; }

        public string AnchorAttributeName { get; set; }

        public bool CanPatch { get; set; }

        public IApiInterfaceObject ApiInterface { get; set; }

        public SchemaType GetSchemaType()
        {
            SchemaType type = SchemaType.Create(this.Name, true);

            foreach (IAttributeAdapter attribute in this.Attributes)
            {
                foreach (SchemaAttribute maAttribute in attribute.GetSchemaAttributes())
                {
                    type.Attributes.Add(maAttribute);
                }
            }

            return type;
        }

        public IEnumerable<string> GetFieldNames(SchemaType type, string api = null)
        {
            foreach (IAttributeAdapter attribute in this.Attributes)
            {
                foreach (string field in attribute.GetFieldNames(type, api))
                {
                    yield return field;
                }
            }
        }
    }
}
