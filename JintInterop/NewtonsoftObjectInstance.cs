using System;
using System.Linq;
using Jint;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Object;
using Jint.Runtime;
using Jint.Runtime.Descriptors;
using Newtonsoft.Json.Linq;

namespace RestNexus.JintInterop
{
    internal sealed class NewtonsoftObjectInstance : ObjectInstance
    {
        private readonly JObject _root;

        public NewtonsoftObjectInstance(Engine engine, JObject root) : base(engine)
        {
            _root = root;
        }

        public override PropertyDescriptor GetOwnProperty(string propertyName)
        {
            var descriptor = base.GetOwnProperty(propertyName);
            if (descriptor == PropertyDescriptor.Undefined)
            {
                var prop = _root.Property(propertyName);
                if (prop != null)
                    Properties[propertyName] = descriptor = new NewtonsoftPropertyDescriptor(this, prop);
            }
            return descriptor;
        }

        internal JsValue Convert(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Array:
                    return Engine.Array.Construct(token.Select(Convert).ToArray());
                case JTokenType.Boolean:
                    return new JsValue(token.Value<bool>());
                case JTokenType.Date:
                    return Engine.Date.Construct(token.Value<DateTime>());
                case JTokenType.Float:
                    return new JsValue(token.Value<float>());
                case JTokenType.Integer:
                    return new JsValue(token.Value<int>());
                case JTokenType.Null:
                    return JsValue.Null;
                case JTokenType.Object:
                    return new NewtonsoftObjectInstance(Engine, (JObject)token);
                case JTokenType.String:
                    return new JsValue(token.Value<string>());
                case JTokenType.Undefined:
                    return JsValue.Undefined;

                case JTokenType.Bytes:
                case JTokenType.Comment:
                case JTokenType.Constructor:
                case JTokenType.Guid:
                case JTokenType.None:
                case JTokenType.Property:
                case JTokenType.Raw:
                case JTokenType.TimeSpan:
                case JTokenType.Uri:
                default:
                    throw new NotSupportedException();
            }
        }

        internal JToken ConvertBack(JsValue value)
        {
            switch (value.Type)
            {
                case Types.Undefined:
                    return JValue.CreateUndefined();
                case Types.Null:
                    return JValue.CreateNull();
                case Types.Boolean:
                    return new JValue(value.AsBoolean());
                case Types.String:
                    return JValue.CreateString(value.AsString());
                case Types.Number:
                    return new JValue(value.AsNumber());
                case Types.Object:
                    if (value.AsObject() is NewtonsoftObjectInstance noi)
                        return noi._root;
                    return new JObject(value.AsObject().GetOwnProperties().Where(kvp => kvp.Value.Enumerable.GetValueOrDefault()).Select(kvp => new JProperty(kvp.Key, ConvertBack(kvp.Value.Value ?? JsValue.Undefined))));

                case Types.None:
                default:
                    throw new NotSupportedException();
            }
        }

        internal JToken ConvertBack(JTokenType type, JsValue value)
        {
            switch (type)
            {
                case JTokenType.Array when value.IsArray():
                    return new JArray(value.AsArray().GetOwnProperties().Where(k => ArrayInstance.IsArrayIndex(new JsValue(k.Key), out _)).Select(kvp => ConvertBack(kvp.Value.Value ?? JsValue.Null)));
                case JTokenType.Boolean when value.IsBoolean():
                    return new JValue(value.AsBoolean());
                case JTokenType.Date when value.IsDate():
                    return new JValue(value.AsDate());
                case JTokenType.Float when value.IsNumber():
                    return new JValue((float)value.AsNumber());
                case JTokenType.Integer when value.IsNumber():
                    return new JValue((int)value.AsNumber());
                case JTokenType.String when value.IsString():
                    return JValue.CreateString(value.AsString());
            }
            return ConvertBack(value);
        }

        private sealed class NewtonsoftPropertyDescriptor : PropertyDescriptor
        {
            private readonly NewtonsoftObjectInstance _parent;
            private readonly JProperty _prop;
            private JsValue _convertedValue;

            public NewtonsoftPropertyDescriptor(NewtonsoftObjectInstance parent, JProperty prop)
            {
                _parent = parent;
                _prop = prop;
                if (_prop.Value != null)
                    _convertedValue = _parent.Convert(_prop.Value);

                Writable = true;
                Configurable = true;
                Enumerable = true;
            }

            public override JsValue Value
            {
                get
                {
                    if (_convertedValue == null && _prop.Value != null)
                        _convertedValue = _parent.Convert(_prop.Value);
                    return _convertedValue;
                }
                set
                {
                    _convertedValue = value;
                    if (value == null)
                        _prop.Value = null;
                    else
                        _prop.Value = _parent.ConvertBack(_prop.Value.Type, value);
                }
            }
        }
    }
}
