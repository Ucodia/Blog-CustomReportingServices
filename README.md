Custom Reporting Services
==

If you have ever used SQL Server Reporting Services (SSRS) you might have been developing reports using Report Builder or Business Intelligence Development Studio (BIDS). Most of time, these tools are just fine for developing reports that fits your needs or your customer requirements. But sometimes you need to build up your own reporting tool, which means you will have to get your hands dirty and inject your own reporting soup into the SSRS engine. The goal of this article serie is to show you the way to building highly customized reporting tool that will fit your needs. These articles mainly targets SSRS 2008 R2 and the Denali CTP3 version and might differ from previous versions.

First stop: the RDL object model. RDL stands for Report Language Defintion. It is a XML based language developed by Microsoft and firstly introduced with the SSRS for SQL Server 2000 release. Every SSRS object you will have to play with are defined with RDL and that is why both the BIDS report designer and Report Builder are using the [Microsoft.ReportingServices.RdlObjectModel][1] namespace to build them up. Now let's have a look on how we can take advantage of this model and go beyond the designers functionalities.

If you wonder how you can generate and manipulate reports you might bump onto this [MSDN article][2]. Basically, all it says is that you can use [XmlDocument][3] or [XmlTextWriter][4] to write your own custom reports. Quite boring and painful. But if BIDS and Report Builder uses a specific model to build up reports, why can't we also take advantage of these functionalities? Of course we can!

### Create a report from scratch

Open up Visual Studio, create a new project and first add references to `Microsoft.ReportingServices.Designer.Controls.dll` and `Microsoft.ReportingServices.RichText.dll`. If you installed BIDS you can find the assembly into your Visual Studio `Common7\IDE\PrivateAssemblies` directory. If you installed Report Builder you can find the asssembly right into the software installation directory. Then create a [Report][5] instance from the `Microsoft.ReportingServices.RdlObjectModel` namespace.

```csharp
Report myReport = new Report();
myReport.Page.PageWidth = new ReportSize(210, SizeTypes.Mm);
myReport.Page.PageHeight = new ReportSize(297, SizeTypes.Mm);
myReport.Width = new ReportSize(210, SizeTypes.Mm);
myReport.Body.Height = new ReportSize(50, SizeTypes.Mm);
```

Here we created the base of our report where we set our page size to be standard a A4 format and our design surface to have the A4 width but with a more reasonable height.

Then we will add a simple [DataSource][6] object. It is not a requirement for the report to have a data source but this is an object you will absolutely require when building dynamic reports.

```csharp
DataSource myDataSource = new DataSource
{
    Name = "myDataSource",
    ConnectionProperties = new ConnectionProperties
    {
        DataProvider = "SQL",
        IntegratedSecurity = true,
        ConnectString = @"Data Source=localhost\SQLSERVER;Initial Catalog=myDatabase"
    }
};
myReport.DataSources.Add(myDataSource);
```

Then you can start adding some basic element to the report body like a [Textbox][7] (yes it is a lower case "b"!).

``` c#
Textbox myTextBox = new Textbox();
myTextBox.Name = "myTextBox";
myTextBox.Paragraphs[0].TextRuns[0].Value = "Hello World";
myReport.Body.ReportItems.Add(myTextBox);
```

Note that whenever you call the [Body][8] property from the Report object, it will return the body of the first [ReportSection][9] of the current report. Also setting the name of your report items is really important. If you forget to do so, you won't be able to run your report and designers will not open it.

### Create a report from template

So here we built a basic report that works and could run on SSRS. But as you have noticed something not really nice is that you have to set the layout manually and it is clearly not an optimized way of building custom reports. So what we will do is create a basic report that can be used as a template.

```xml
<?xml version="1.0" encoding="utf-8"?>
<Report xmlns:rd="http://schemas.microsoft.com/SQLServer/reporting/reportdesigner" xmlns:cl="http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition" xmlns="http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition">
  <AutoRefresh>0</AutoRefresh>
  <DataSources>
    <DataSource Name="myDataSource">
      <ConnectionProperties>
        <DataProvider>SQL</DataProvider>
        <ConnectString>Data Source=localhost\SQLSERVER;Initial Catalog=myDatabase</ConnectString>
        <IntegratedSecurity>true</IntegratedSecurity>
      </ConnectionProperties>
      <rd:SecurityType>Integrated</rd:SecurityType>
      <rd:DataSourceID>b0ed98bc-cc6f-4636-8e4b-9ebadbd5b6e2</rd:DataSourceID>
    </DataSource>
  </DataSources>
  <ReportSections>
    <ReportSection>
      <Body>
        <Height>50mm</Height>
        <Style>
          <Border>
            <Style>None</Style>
          </Border>
        </Style>
      </Body>
      <Width>210mm</Width>
      <Page>
        <PageHeight>29.7cm</PageHeight>
        <PageWidth>21cm</PageWidth>
        <LeftMargin>2cm</LeftMargin>
        <RightMargin>2cm</RightMargin>
        <TopMargin>2cm</TopMargin>
        <BottomMargin>2cm</BottomMargin>
        <ColumnSpacing>0.13cm</ColumnSpacing>
        <Style />
      </Page>
    </ReportSection>
  </ReportSections>
  <rd:ReportUnitType>Mm</rd:ReportUnitType>
  <rd:ReportID>21e8391f-35bc-4a68-bdc6-6c9f183653e8</rd:ReportID>
</Report>
```

You can download the full template [here][10].

In this template we only set the page size, the design surface size and added the data source. To get the exact same object as before we only have to instantiate the report from the template and add the `Textbox`.

```csharp
Report myReport = Report.Load("TemplateA4.rdl");

Textbox myTextBox = new Textbox();
myTextBox.Name = "myTextBox";
myTextBox.Paragraphs[0].TextRuns[0].Value = "Hello World";

myReport.Body.ReportItems.Add(myTextBox);
```

As you can see it is a bunch of somewhat boring and hard coded instructions we saved here.

You are now ready to build customized reporting tool that gives you full control on how the report will be generated. I strongly suggest to build some business relevant reports and then analyse the XML structure to get more familiar with the most common RDL objects. This will give you a better understanding on how you can extend the designer capabilities and shape the report generation.

But now we are still missing a crucial part: how are we gonna serialize the report to a RDL file? Well. The answer is pretty simple: use the `RdlSerializer` class. Now if you search for the class documentation, you won't find anything. If in the end you start looking into the API code using [JustDecompile][11], you would bump into this `RdlSerializer` class and finally realise it is internal...

The reason why Microsoft decided to not put it in the public API is still a bit unclear to me. Being inspired by [Teo Lachev's post][12] about the SSRS 2008 R2 object model I decided to write a `RdlSerializer` proxy class hacking into the original internal class.

### Here comes the RdlSerializer

To make this proxy class as fast as possible on initalisation and invocation, I used a lazy-loading pattern for method reflection. Here is an excerpt.

```csharp
public class RdlSerializer
{
    private Type internalType;
    private object typeInstance;

    /* ... */

    private MethodInfo serializeToXmlWriterMethod;

    public RdlSerializer()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(Report));

        this.typeInstance = assembly.CreateInstance("Microsoft.ReportingServices.RdlObjectModel.Serialization.RdlSerializer");
        this.internalType = this.typeInstance.GetType();
    }

    /* ... */

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
```

You can view the full class [here][13].

Now you have the power of the `RdlSerializer` by your side, you can serialize and persist your programmatically generated reports. If we reuse the code from my previous article, it would give something like this.

```csharp
RdlSerializer serializer = new RdlSerializer();
Report myReport = Report.Load("TemplateA4.rdl");

Textbox myTextBox = new Textbox();
myTextBox.Name = "myTextBox";
myTextBox.Paragraphs[0].TextRuns[0].Value = "Hello World";

myReport.Body.ReportItems.Add(myTextBox);

StreamWriter writer = new StreamWriter("CustomReport.rdl");
Report myCustomReport = serializer.Serialize(writer, myReport);
```

Pretty intuitive.

What's next then? Build your tools! But, I warn you, if you really want to develop custom reporting tools, be sure of the deployment strategy you will use. Just have a look at the build output to understand what I mean.

### A Hell of dependencies

As Teo Lachev states in his article, the RDL object model includes no less than 25 assemblies in which at least 20 of them are required to run any of the previous bit of code we wrote. This gave me a clue on why the `RdlSerializer` has never been pushed to the public API. It was not meant to be redistribuable. All we can hope is that the SQL Server team had plans in making it easy to deploy for its SQL Server Denali RTM. Even if I haven't heard or read of plans concerning this issue, you can track [this feature request][14] on Microsoft Connect website.

  [1]: http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.aspx
  [2]: http://msdn.microsoft.com/en-us/library/ms170667.aspx
  [3]: http://msdn.microsoft.com/en-us/library/system.xml.xmldocument.aspx
  [4]: http://msdn.microsoft.com/en-us/library/system.xml.xmltextwriter.aspx
  [5]: http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.report.aspx
  [6]: http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.datasource.aspx
  [7]: http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.textbox.aspx
  [8]: href=%22http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.report.body.aspx
  [9]: http://msdn.microsoft.com/en-us/library/microsoft.reportingservices.rdlobjectmodel.reportsection.aspx
  [10]: code/A4Template.rdl
  [11]: http://www.telerik.com/products/decompiler.aspx
  [12]: http://prologika.com/CS/blogs/blog/archive/2010/03/08/where-is-rdlom-in-r2.aspx
  [13]: code/RdlSerializer.cs
  [14]: https://connect.microsoft.com/SQLServer/feedback/details/540183/supported-rdl-object-model-rdlom
