public class RdlSerializer
{
    private Type internalType;
    private object typeInstance;

    private MethodInfo deserializeStreamToReportMethod;
    private MethodInfo deserializeTextReaderToReportMethod;
    private MethodInfo deserializeXmlReaderToReportMethod;
    private MethodInfo deserializeStreamToObjectMethod;
    private MethodInfo deserializeTextReaderToObjectMethod;
    private MethodInfo deserializeXmlReaderToObjectMethod;
    private MethodInfo serializeToStreamMethod;
    private MethodInfo serializeToTextWriterMethod;
    private MethodInfo serializeToXmlWriterMethod;

    public RdlSerializer()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Report));

        this.typeInstance = assembly.CreateInstance("Microsoft.ReportingServices.RdlObjectModel.Serialization.RdlSerializer");
        this.internalType = this.typeInstance.GetType();
    }

    public Report Deserialize(Stream stream)
    {
        if (this.deserializeStreamToReportMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(Stream)
            };

            this.deserializeStreamToReportMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            stream
        };

        return (Report)this.deserializeStreamToReportMethod.Invoke(this.typeInstance, methodParameters);
    }

    public Report Deserialize(TextReader textReader)
    {
        if (this.deserializeTextReaderToReportMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(TextReader)
            };

            this.deserializeTextReaderToReportMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            textReader
        };

        return (Report)this.deserializeTextReaderToReportMethod.Invoke(this.typeInstance, methodParameters);
    }

    public Report Deserialize(XmlReader xmlReader)
    {
        if (this.deserializeXmlReaderToReportMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(XmlReader)
            };

            this.deserializeXmlReaderToReportMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            xmlReader
        };

        return (Report)this.deserializeXmlReaderToReportMethod.Invoke(this.typeInstance, methodParameters);
    }

    public object Deserialize(Stream stream, Type objectType)
    {
        if (this.deserializeStreamToObjectMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(Stream),
                typeof(Type)
            };

            this.deserializeStreamToObjectMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            stream,
            objectType
        };

        return this.deserializeStreamToObjectMethod.Invoke(this.typeInstance, methodParameters);
    }

    public object Deserialize(TextReader textReader, Type objectType)
    {
        if (this.deserializeTextReaderToObjectMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(TextReader),
                typeof(Type)
            };

            this.deserializeTextReaderToObjectMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            textReader,
            objectType
        };

        return this.deserializeTextReaderToObjectMethod.Invoke(this.typeInstance, methodParameters);
    }

    public object Deserialize(XmlReader xmlReader, Type objectType)
    {
        if (this.deserializeXmlReaderToObjectMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(XmlReader),
                typeof(Type)
            };

            this.deserializeXmlReaderToObjectMethod = this.internalType.GetMethod("Deserialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            xmlReader,
            objectType
        };

        return this.deserializeXmlReaderToObjectMethod.Invoke(this.typeInstance, methodParameters);
    }

    public void Serialize(Stream stream, object o)
    {
        if (this.serializeToStreamMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(Stream),
                typeof(object)
            };

            this.serializeToStreamMethod = this.internalType.GetMethod("Serialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            stream,
            o
        };

        this.serializeToStreamMethod.Invoke(this.typeInstance, methodParameters);
    }

    public void Serialize(TextWriter textWriter, object o)
    {
        if (this.serializeToTextWriterMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(TextWriter),
                typeof(object)
            };

            this.serializeToTextWriterMethod = this.internalType.GetMethod("Serialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            textWriter,
            o
        };

        this.serializeToTextWriterMethod.Invoke(this.typeInstance, methodParameters);
    }

    public void Serialize(XmlWriter xmlWriter, object o)
    {
        if (this.serializeToXmlWriterMethod == null)
        {
            Type[] methodSignature = new Type[]
            {
                typeof(XmlWriter),
                typeof(object)
            };

            this.serializeToXmlWriterMethod = this.internalType.GetMethod("Serialize", methodSignature);
        }

        object[] methodParameters = new object[]
        {
            xmlWriter,
            o
        };

        this.serializeToXmlWriterMethod.Invoke(this.typeInstance, methodParameters);
    }
}