using Application.Logic;
using Application.LogicInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Utils;

namespace Testing.WebApiTests;

[TestClass]
public class ConverterTest : DbTestBase
{
    private IConverter _convert; 
    
    
    // public ConverterTest()
    // {
    //     _convert = new Converter();
    // }

    [TestMethod]
    public void THCPayloadRead()
    {
        _convert.ConvertFromHex("07800c9401e0");
    }
}