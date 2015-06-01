using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for TypeConverterTest and is intended
    ///to contain all TypeConverterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TypeConverterTest
    {
        [TestMethod()]
        public void ConvertDataDateTimeFromStringTest1()
        {
            object obj = "2000-02-01 5:36:11.987";
            object actual;
            actual = TypeConverter.ConvertData<DateTime>(obj);

            if (!(actual is DateTime))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else 
            {
                DateTime actualValue = (DateTime)actual;
                if (actualValue.Year != 2000)
                {
                    Assert.Fail("The conversion return the incorrect year");
                }

                if (actualValue.Month != 2)
                {
                    Assert.Fail("The conversion return the incorrect month");
                }

                if (actualValue.Day != 1)
                {
                    Assert.Fail("The conversion return the incorrect day");
                }

                if (actualValue.Hour != 5)
                {
                    Assert.Fail("The conversion return the incorrect hour");
                }

                if (actualValue.Minute != 36)
                {
                    Assert.Fail("The conversion return the incorrect minute");
                }
                
                if (actualValue.Second != 11)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }

                if (actualValue.Millisecond != 987)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }
            }
        }

        [TestMethod()]
        public void ConvertDataDateTimeFromStringTest2()
        {
            object obj = "2000-02-01T05:36:11.987";
            object actual;
            actual = TypeConverter.ConvertData<DateTime>(obj);

            if (!(actual is DateTime))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else
            {
                DateTime actualValue = (DateTime)actual;
                if (actualValue.Year != 2000)
                {
                    Assert.Fail("The conversion return the incorrect year");
                }

                if (actualValue.Month != 2)
                {
                    Assert.Fail("The conversion return the incorrect month");
                }

                if (actualValue.Day != 1)
                {
                    Assert.Fail("The conversion return the incorrect day");
                }

                if (actualValue.Hour != 5)
                {
                    Assert.Fail("The conversion return the incorrect hour");
                }

                if (actualValue.Minute != 36)
                {
                    Assert.Fail("The conversion return the incorrect minute");
                }

                if (actualValue.Second != 11)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }

                if (actualValue.Millisecond != 987)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }
            }
        }

        [TestMethod()]
        public void ConvertDataDateTimeFromStringTest3()
        {
            object obj = "1/2/2000 05:36:11.987";
            object actual;
            actual = TypeConverter.ConvertData<DateTime>(obj);

            if (!(actual is DateTime))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else
            {
                DateTime actualValue = (DateTime)actual;
                if (actualValue.Year != 2000)
                {
                    Assert.Fail("The conversion return the incorrect year");
                }

                if (actualValue.Month != 2)
                {
                    Assert.Fail("The conversion return the incorrect month");
                }

                if (actualValue.Day != 1)
                {
                    Assert.Fail("The conversion return the incorrect day");
                }

                if (actualValue.Hour != 5)
                {
                    Assert.Fail("The conversion return the incorrect hour");
                }

                if (actualValue.Minute != 36)
                {
                    Assert.Fail("The conversion return the incorrect minute");
                }

                if (actualValue.Second != 11)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }

                if (actualValue.Millisecond != 987)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }
            }
        }

        [TestMethod()]
        public void ConvertDataDateTimeFromTicksTest()
        {
            object obj = DateTime.Parse("1/2/2000 05:36:11.987").Ticks;
            object actual;
            actual = TypeConverter.ConvertData<DateTime>(obj);

            if (!(actual is DateTime))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else
            {
                DateTime actualValue = (DateTime)actual;
                if (actualValue.Year != 2000)
                {
                    Assert.Fail("The conversion return the incorrect year");
                }

                if (actualValue.Month != 2)
                {
                    Assert.Fail("The conversion return the incorrect month");
                }

                if (actualValue.Day != 1)
                {
                    Assert.Fail("The conversion return the incorrect day");
                }

                if (actualValue.Hour != 5)
                {
                    Assert.Fail("The conversion return the incorrect hour");
                }

                if (actualValue.Minute != 36)
                {
                    Assert.Fail("The conversion return the incorrect minute");
                }

                if (actualValue.Second != 11)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }

                if (actualValue.Millisecond != 987)
                {
                    Assert.Fail("The conversion return the incorrect seconds");
                }
            }
        }

        [TestMethod()]
        public void ConvertDataDateTimeFromNullTest()
        {
            object obj = null;
            object actual;
            try
            {
                actual = TypeConverter.ConvertData<DateTime>(obj);
                Assert.Fail("The conversion did not throw an exception when passed a null value");
            }
            catch
            {
            }
        }

        [TestMethod()]
        public void ConvertDataLongFromStringTest()
        {
            object obj = "66";
            object actual;
            actual = TypeConverter.ConvertData<long>(obj);

            if (!(actual is long))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((long)actual != 66L)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataLongFromIntegerTest()
        {
            object obj = 66;
            object actual;
            actual = TypeConverter.ConvertData<long>(obj);

            if (!(actual is long))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((long)actual != 66L)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataLongFromLongTest()
        {
            object obj = 66L;
            object actual;
            actual = TypeConverter.ConvertData<long>(obj);

            if (!(actual is long))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((long)actual != 66L)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataLongFromNullTest()
        {
            object obj = null;
            object actual;
            try
            {
                actual = TypeConverter.ConvertData<long>(obj);
                Assert.Fail("The conversion did not throw an exception when passed a null value");
            }
            catch 
            {
            }
        }

        [TestMethod()]
        public void ConvertDataBinaryFromStringTest()
        {
            object obj = "AAECAwQ=";
            object actual;
            actual = TypeConverter.ConvertData<byte[]>(obj);

            if (!(actual is byte[]))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if (!((byte[])actual).SequenceEqual(new byte[] { 0, 1, 2, 3, 4 })) 
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBinaryFromBinaryTest()
        {
            object obj = new byte[] { 0, 1, 2, 3, 4 };
            object actual;
            actual = TypeConverter.ConvertData<byte[]>(obj);

            if (!(actual is byte[]))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if (!((byte[])actual).SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBinaryFromNullTest()
        {
            object obj = null;
            object actual;
            actual = TypeConverter.ConvertData<byte[]>(obj);

            if (actual != null)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBooleanFromStringTest()
        {
            object obj = "true";
            object actual;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != true)
            {
                Assert.Fail("The conversion failed");
            }

            obj = "false";

            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != false)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBooleanFromIntegerTest()
        {
            object obj = 1;
            object actual;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != true)
            {
                Assert.Fail("The conversion failed");
            }

            obj = 0L;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != false)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBooleanFromLongTest()
        {
            object obj = 1L;
            object actual;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != true)
            {
                Assert.Fail("The conversion failed");
            }

            obj = 0L;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != false)
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataBooleanFromBooleanTest()
        {
            object obj = true;
            object actual;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != true)
            {
                Assert.Fail("The conversion failed");
            }

            obj = false;
            actual = TypeConverter.ConvertData<bool>(obj);

            if (!(actual is bool))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((bool)actual != false)
            {
                Assert.Fail("The conversion failed");
            }
        }
        
        [TestMethod()]
        public void ConvertDataBooleanFromNullTest()
        {
            object obj = null;
            object actual;
            try
            {
                actual = TypeConverter.ConvertData<bool>(obj);
                Assert.Fail("The conversion did not throw an exception when passed a null value");
            }
            catch
            {
            }
        }

        [TestMethod()]
        public void ConvertDataGuidFromStringTest()
        {
            object obj = "{5a405911-f15d-4d49-9d74-a5ebeb273f57}";
            object actual;
            actual = TypeConverter.ConvertData<Guid>(obj);

            if (!(actual is Guid))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((Guid)actual != new Guid("{5a405911-f15d-4d49-9d74-a5ebeb273f57}")) 
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataGuidFromGuidTest()
        {
            object obj = new Guid("{5a405911-f15d-4d49-9d74-a5ebeb273f57}");
            object actual;
            actual = TypeConverter.ConvertData<Guid>(obj);

            if (!(actual is Guid))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((Guid)actual != new Guid("{5a405911-f15d-4d49-9d74-a5ebeb273f57}"))
            {
                Assert.Fail("The conversion failed");
            }
        }
        
        [TestMethod()]
        public void ConvertDataGuidFromNullTest()
        {
            object obj = null;
            object actual;
            try
            {
                actual = TypeConverter.ConvertData<Guid>(obj);
                Assert.Fail("The conversion did not throw an exception when passed a null value");
            }
            catch
            {
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromStringTest()
        {
            object obj = "test";
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "test")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromGuidTest()
        {
            object obj = new Guid("2eea7c52-08f8-46a4-9517-4009d2582ffe");
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "2eea7c52-08f8-46a4-9517-4009d2582ffe")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromLongTest()
        {
            object obj = 55L;
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "55")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromIntTest()
        {
            object obj = 55;
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "55")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromBooleanTest()
        {
            object obj = true;
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "True")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromBinaryTest()
        {
            object obj = new byte[] { 0, 1, 2, 3, 4 };
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (!(actual is string))
            {
                Assert.Fail("The conversion returned the wrong data type");
            }
            else if ((string)actual != "AAECAwQ=")
            {
                Assert.Fail("The conversion failed");
            }
        }

        [TestMethod()]
        public void ConvertDataStringFromNullTest()
        {
            object obj = null;
            object actual;
            actual = TypeConverter.ConvertData<string>(obj);

            if (actual != null)
            {
                Assert.Fail("The conversion failed");
            }
        }
    }
}
