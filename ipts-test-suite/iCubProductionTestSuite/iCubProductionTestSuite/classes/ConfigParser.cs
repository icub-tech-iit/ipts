/*
* Copyright (C) 2018 Istituto Italiano di Tecnologia
* Authors: davide.tome@iit.it
* CopyPolicy: Released under the terms of the LGPLv2.1 or later, see LGPL.TXT
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace iCubProductionTestSuite.classes
{
    internal class ConfigParser
    {
        private SettingsFile sf;
        private ConfigFile conf;
        private Testplan tp;
        private List<Testplan> testplanlist;

        public SettingsFile readSettings(String file)
        {
            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;
            XmlElement el;
            sf = new SettingsFile();
            
            doc.Load(file);

            el = doc.DocumentElement;

            sf.LastSel = el.Attributes["lastSel"].Value;
            sf.User = el.Attributes["operator"].Value;

            nodes = el.ChildNodes;

            if (nodes.Count == 0)
            {
                throw new AppException("Il formato del file " + file + " non e'corretto");
            }
            
            foreach (XmlNode n in nodes)
            {
                SettingsFile.Setting s = new SettingsFile.Setting();

                s.TestplanId = n.Attributes["testplan"].Value;
                s.LastSN = n.Attributes["serial"].Value;

                sf.Settings.Add(s);
            }
            return sf;
        }

        public void writeSettings(String file, String user, String seltp, String serial)
        {
            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;
            XmlElement el;
            sf = new SettingsFile();
            
            doc.Load(file);

            el = doc.DocumentElement;

            el.SetAttribute("lastSel", seltp);
            el.SetAttribute("operator", user);

            nodes = el.ChildNodes;

            foreach (XmlNode n in nodes)
            {
                SettingsFile.Setting s = new SettingsFile.Setting();
                if(n.Attributes["testplan"].Value.Equals(seltp)) n.Attributes["serial"].Value = serial;
            }

            doc.Save(file);

        }

        public void readStartupFromConfig(String file)
        {
            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;

            conf = new ConfigFile();
            
            doc.Load(file);

            nodes = doc.DocumentElement.SelectNodes("/iCubProductionTestSuite/startup");

            if (nodes.Count == 0)
            {
                return;
            }

            foreach (XmlNode n in nodes)
            {
               switch(n.Attributes["type"].Value)
                {
                    case "batch":
                        Operation op = new Operation();
                        op.Type = "batch";
                        op.BatchDir = n.Attributes["batchDir"].Value;
                        op.BatchFile = n.Attributes["batchFile"].Value;
                        if (n.Attributes["errorMessage"] != null) op.ErrorMessage = n.Attributes["errorMessage"].Value;
                        if (n.Attributes["noFail"] != null && n.Attributes["noFail"].Value == "true") op.NoFail = true;
                        else op.NoFail = false;
                        
                        CommandRunner cr = new CommandRunner(op);
                        bool res = cr.runBatch();
                        if(!res && !op.NoFail) throw new AppException(op.ErrorMessage);
                        break;

                    default: break;
                }
            }
        }

        public List<Testplan> readBoardsFromConfig(String file)
        {
            XmlDocument doc = new XmlDocument();
            XmlNodeList nodes;

            conf = new ConfigFile();
            testplanlist = new List<Testplan>();
            
            doc.Load(file);

            nodes = doc.DocumentElement.SelectNodes("/iCubProductionTestSuite/testplan");

            if (nodes.Count == 0)
            {
                throw new AppException("Il formato del file " + file + " non e'corretto");
            }

            foreach (XmlNode n in nodes)
            {
                Testplan t = new Testplan();

                t.Idtestplan = n.Attributes["id"].Value;
                t.Boardname = n.Attributes["board"].Value;
                t.Iitcode = n.Attributes["iitcode"].Value;
                t.Rev = n.Attributes["rev"].Value;
                t.ReportsDir = n.Attributes["reportsDir"].Value;

                testplanlist.Add(t);
            }

            return testplanlist;
        }

        public Testplan readConfig(String file, String id)
        {
            //carico il file config.xml
            //estraggo i nodi interfaces,programmers,board,test,operation
            //e creo un oggetto contenente tutte le info
            XmlDocument doc = new XmlDocument();
            tp = new Testplan();
            XmlNodeList nodes_tp, nodes_test, testinterfaces, testinterface;
            int tp_index = 0;
            int k = 0;

            tp = new Testplan();
            
            doc.Load(file);

            nodes_tp = doc.DocumentElement.SelectNodes("/iCubProductionTestSuite/testplan[@id='" + id + "']");

            if (nodes_tp.Count == 0)
            {
                throw new AppException("Il formato del file " + file + " non e'corretto");
            }

            //seleziono il nodo testplan
            foreach (XmlNode tp in nodes_tp)
            {
                string s = tp.Attributes["id"].Value;
                if (tp.Attributes["id"].Value.Equals(id)) tp_index = k;
                k++;
            }

            tp.Idtestplan = nodes_tp[tp_index].Attributes["id"].Value;
            tp.Boardname = nodes_tp[tp_index].Attributes["board"].Value;
            tp.Iitcode = nodes_tp[tp_index].Attributes["iitcode"].Value;
            tp.Rev = nodes_tp[tp_index].Attributes["rev"].Value;
            tp.ReportsDir = nodes_tp[tp_index].Attributes["reportsDir"].Value;


            //PARSING TEST INTERFACES//
            testinterfaces = nodes_tp[tp_index].SelectNodes("interfaces");
/*
            if (testinterfaces.Count == 0)
            {
                throw new AppException("Il formato del file " + file + " non e'corretto");
            }
*/
            foreach (XmlNode node in testinterfaces)
            {
             

                testinterface = node.SelectNodes("interface");

                if (testinterface.Count == 0)
                {
                    throw new AppException("Il formato del file " + file + " non e'corretto");
                }

                foreach (XmlNode n in testinterface)
                {
                    TestInterface iface = new TestInterface();

                    XmlAttributeCollection attributes = n.Attributes;

                    foreach (XmlAttribute a in attributes)
                    {
                        switch (a.Name)
                        {
                            case "name":
                                iface.Name = n.Attributes["name"].Value;
                                break;

                            case "netPort":
                                iface.NetPort = n.Attributes["netPort"].Value;
                                break;

                            case "bitrates":
                                string[] bs = n.Attributes["bitrates"].Value.Split(',');
                                foreach (string s in bs)
                                {
                                    iface.Bitrates.Add(s);
                                }
                                break;

                            case "bitrate":
                                iface.Bitrate = n.Attributes["bitrate"].Value;
                                break;

                            case "messageID":
                                iface.MessageID = n.Attributes["messageID"].Value;
                                break;

                            default: break;
                        }
                    }
                    tp.TestInterfaces.Add(iface);
                }
                
            }

            // PARSING TESTS
            nodes_test = nodes_tp[tp_index].SelectNodes("test");

            if (nodes_test.Count == 0)
            {
                throw new AppException("Il formato del file " + file + " non e'corretto");
            }

            foreach (XmlNode node1 in nodes_test)
            {
                XmlNodeList ops;
                Test test = new Test();

                test.Id = node1.Attributes["id"].Value;
                test.Name = node1.Attributes["name"].Value;

                ops = node1.SelectNodes("operation");

                if (ops.Count == 0)
                {
                    throw new AppException("Il formato del file " + file + " non e'corretto");
                }

                foreach (XmlNode op in ops)
                {
                    Operation operation = new Operation();

                    XmlAttributeCollection attributes = op.Attributes;

                    foreach (XmlAttribute a in attributes)
                    {
                        switch (a.Name)
                        {
                            case "type":
                                operation.Type = op.Attributes["type"].Value;
                                break;

                            case "text":
                                operation.Text = op.Attributes["text"].Value;
                                break;

                            case "interface":
                                operation.Interf = op.Attributes["interface"].Value;
                                break;

                            case "image":
                                operation.Image = op.Attributes["image"].Value;
                                break;

                            case "var":
                                operation.Var = op.Attributes["var"].Value;
                                break;

                            case "varType":
                                operation.Vartype = op.Attributes["varType"].Value;
                                break;

                            case "appendVar":
                                operation.AppendVar = op.Attributes["appendVar"].Value;
                                break;

                            case "lenght":
                                operation.Lenght = op.Attributes["lenght"].Value;
                                break;

                            case "compareToVal":
                                operation.Comparetovar = op.Attributes["compareToVar"].Value;
                                break;

                            case "command":
                                operation.Command = op.Attributes["command"].Value;
                                break;

                            case "value":
                                operation.Value = op.Attributes["value"].Value;
                                break;

                            case "log":
                                operation.Log = op.Attributes["log"].Value;
                                break;

                            case "logMess":
                                operation.LogMess = op.Attributes["logMess"].Value;
                                break;

                            case "batchDir":
                                operation.BatchDir = op.Attributes["batchDir"].Value;
                                break;

                            case "batchFile":
                                operation.BatchFile = op.Attributes["batchFile"].Value;
                                break;

                            case "valPass":
                                operation.ValPass = op.Attributes["valPass"].Value;
                                break;
                            case "errorMessage":
                                operation.ErrorMessage = op.Attributes["errorMessage"].Value;
                                break;

                            default: break;
                        }
                    }

                    test.OperationList.Add(operation);
                }

                tp.TestList.Add(test);
            }

            return tp;
        }
    }

    public class SettingsFile
    {
        private String lastSel;
        private List<Setting> settings;
        private String user;

        public SettingsFile()
        {
            settings = new List<Setting>();
        }

        public string LastSel
        {
            get
            {
                return lastSel;
            }

            set
            {
                lastSel = value;
            }
        }

        public List<Setting> Settings
        {
            get
            {
                return settings;
            }

            set
            {
                settings = value;
            }
        }

        public string User
        {
            get
            {
                return user;
            }

            set
            {
                user = value;
            }
        }

        public class Setting
        {
            private String testplanId;
            private String lastSN;

            public string TestplanId
            {
                get
                {
                    return testplanId;
                }

                set
                {
                    testplanId = value;
                }
            }

           
            public string LastSN
            {
                get
                {
                    return lastSN;
                }

                set
                {
                    lastSN = value;
                }
            }
        }

      
    }
        public class ConfigFile
    {
        private String idtestplan;
        private String boardname;
        private String iitcode;
        private String rev;
        private String reportsDir;
        private List<Test> testList;
        private List<TestInterface> testInterfaces;
        private List<Testplan> testplanlist;

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

        internal List<TestInterface> TestInterfaces
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

        public String Idtestplan
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

      
        internal List<Testplan> Testplanlist
        {
            get
            {
                return testplanlist;
            }

            set
            {
                testplanlist = value;
            }
        }

        public ConfigFile()
        {
            TestInterfaces = new List<TestInterface>();
            TestList = new List<Test>();
        }
    }

    public class Test
    {
        private String id;
        private String name;
        private bool result;
        private List<Operation> operationList;

        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        internal List<Operation> OperationList
        {
            get
            {
                return operationList;
            }

            set
            {
                operationList = value;
            }
        }

        public bool Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        public Test()
        {
            OperationList = new List<Operation>();
        }
    }

    internal class Operation
    {
        private String type;
        private String text;
        private String image;
        private String interf;
        private String var;
        private String vartype;
        private String appendVar;
        private String lenght;
        private String comparetovar;
        private String toll;
        private String command;
        private String value;
        private String log;
        private String logMess;
        private String batchDir;
        private String batchFile;
        private String valPass;
        private String errorMessage;
        private bool noFail;

        public String Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public String Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        public String Image
        {
            get
            {
                return image;
            }

            set
            {
                image = value;
            }
        }

        public String Interf
        {
            get
            {
                return interf;
            }

            set
            {
                interf = value;
            }
        }

        public String Var
        {
            get
            {
                return Var1;
            }

            set
            {
                Var1 = value;
            }
        }

        public String Vartype
        {
            get
            {
                return vartype;
            }

            set
            {
                vartype = value;
            }
        }

        public String Comparetovar
        {
            get
            {
                return comparetovar;
            }

            set
            {
                comparetovar = value;
            }
        }

        public String Toll
        {
            get
            {
                return toll;
            }

            set
            {
                toll = value;
            }
        }

        public String Command
        {
            get
            {
                return command;
            }

            set
            {
                command = value;
            }
        }

        public String Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        public String Var1
        {
            get
            {
                return var;
            }

            set
            {
                var = value;
            }
        }

        public String AppendVar
        {
            get
            {
                return appendVar;
            }

            set
            {
                appendVar = value;
            }
        }

        public String Lenght
        {
            get
            {
                return lenght;
            }

            set
            {
                lenght = value;
            }
        }

        public String ValPass
        {
            get
            {
                return valPass;
            }

            set
            {
                valPass = value;
            }
        }

        public String LogMess
        {
            get
            {
                return logMess;
            }

            set
            {
                logMess = value;
            }
        }

        public string BatchDir
        {
            get
            {
                return batchDir;
            }

            set
            {
                batchDir = value;
            }
        }

        public string BatchFile
        {
            get
            {
                return batchFile;
            }

            set
            {
                batchFile = value;
            }
        }

        public string Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        public bool NoFail
        {
            get
            {
                return noFail;
            }

            set
            {
                noFail = value;
            }
        }
    }
}