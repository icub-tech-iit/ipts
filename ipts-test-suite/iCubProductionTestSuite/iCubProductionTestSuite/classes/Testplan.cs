using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iCubProductionTestSuite.classes
{
    public class Testplan
    {
        private List<TestItemControl> tests;
        private String idtestplan;
        private String boardname;
        private String iitcode;
        private String rev;
        private String reportsDir;
        public List<Test> testList;
        private List<TestInterface> testInterfaces;

        public Testplan()
        {
            Tests = new List<TestItemControl>();
            TestInterfaces = new List<TestInterface>();
            TestList = new List<Test>();
        }

        public List<TestItemControl> Controls
        {
            get
            {
                return Tests;
            }
        }

        public void addControl(TestItemControl c)
        {
            Controls.Add(c);
        }

        public List<Test> TestPlan
        {
            get
            {
                List<Test> l = new List<Test>();

                foreach (TestItemControl c in Tests)
                    if (c.CheckboxChecked)
                        foreach (Test t in testList)
                            if (t.Name.Equals(c.CheckboxText)) l.Add(t);

                return l;
            }
        }

        public List<TestItemControl> Tests
        {
            get
            {
                return tests;
            }

            set
            {
                tests = value;
            }
        }

        public string Idtestplan
        {
            get
            {
                return idtestplan;
            }

            set
            {
                idtestplan = value;
            }
        }

        public string Boardname
        {
            get
            {
                return boardname;
            }

            set
            {
                boardname = value;
            }
        }

        public string Iitcode
        {
            get
            {
                return iitcode;
            }

            set
            {
                iitcode = value;
            }
        }

        public string Rev
        {
            get
            {
                return rev;
            }

            set
            {
                rev = value;
            }
        }

        public string ReportsDir
        {
            get
            {
                return reportsDir;
            }

            set
            {
                reportsDir = value;
            }
        }

        internal List<Test> TestList
        {
            get
            {
                return testList;
            }

            set
            {
                testList = value;
            }
        }

        public List<TestInterface> TestInterfaces
        {
            get
            {
                return testInterfaces;
            }

            set
            {
                testInterfaces = value;
            }
        }

        public void setTestResult(int index, string result)
        {
            Tests[index].setResult(result);
        }

        public void resetTestResult()
        {
            foreach(TestItemControl t in Tests)  if(t.CheckboxChecked) t.resetResult();
        }
    }
}
