using System;
using System.Data;
using System.Collections.Generic;
using System.IO;

public class Mobile
{
    protected string phoneState;

    public string PhoneNumber { get; set; }
    public virtual string PhoneState { get=>"State.normal"; set=>phoneState="State.normal"; }
    public string ConnectionState { get; set; }

    public string OS {get; set; }

    public Mobile(string number)
    {
        PhoneNumber=number;
        PhoneState="State.normal";
        ConnectionState="offline";
    }

    public void receiveACall(string incommingNumber) { 
        System.Console.WriteLine($"Calling from {incommingNumber}");
    }

    public void ringAnAlarm() { 
        System.Console.WriteLine($"Ringing an alarm on {PhoneNumber}");
    }

}
 
public class Smartphone: Mobile
{
    public Smartphone(string number):base(number)
    {
    }
    public override string PhoneState { get=>phoneState; set=>phoneState=value; }
    public string Position{get;set;}

}
public class KommunikationsKoordinator
{
    private List<Mobile> instanzen;
    public List<Mobile> Instanzen{get=>instanzen;}

    public KommunikationsKoordinator()
    {
        instanzen= new List<Mobile>();
    }

    
    public void ReadData(string filename)
    {
        String line;
        try
        {
    
            StreamReader sr = new StreamReader(filename);
            line = sr.ReadLine();
            while (line != null)
             {
                String[] teile=line.Split(',');
                foreach (var str in teile) Console.Write(str+"\t"); System.Console.WriteLine();
                if (teile[1].Equals("OS_A")) 
                {
                    Mobile mobile=new Mobile(teile[0]);
                    mobile.PhoneState=teile[2];mobile.ConnectionState=teile[3];
                    instanzen.Add(mobile);
                }
                if (teile[1].Equals("OS_B")) 
                {
                    Smartphone mobile=new Smartphone(teile[0]);
                    mobile.PhoneState=teile[2];mobile.ConnectionState=teile[3];
                    mobile.Position=teile[4]+","+teile[5]; 
                    instanzen.Add(mobile);
                }

                line = sr.ReadLine();
            }
            sr.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }
    }
    public bool TestAnruf(string von, string an)
    { 
        //die Nummer "von" ruft die Nummer "an" 
        try{
            Mobile item_von=null, item_an=null;
            //beide Nummern werden in der Liste gesucht 
            foreach(var item in instanzen)
            {
                if (item.PhoneNumber.Equals(von))
                {
                    item_von=item;
                }
                else 
                    if (item.PhoneNumber.Equals(an))
                    {
                        item_an=item;
                    }
            }
            if (item_von.ConnectionState.Equals("offline"))  return false;
            if (item_an.ConnectionState.Equals("offline"))  return false;
            item_an.receiveACall(von);//Aufruf der Methode zum Empfangen
        }
        catch(Exception )
        {
            return false;
        }
        return true;
    }
    bool TestAlarm()
    {
        return false;
    }
    public bool TestGetPosition()
    {
        return false;
    }
}

    class MobilfunkSimulation
    {
        private KommunikationsKoordinator kommunikationsKoordinator;
        private List<string> tests;
        public MobilfunkSimulation()
        {
            kommunikationsKoordinator=new KommunikationsKoordinator();
            tests=new List<string>();  
        }

        public void ReadTests(string filename)
        {
            String line;
                try
                {
            
                    StreamReader sr = new StreamReader(filename);
                    line = sr.ReadLine();
                    while (line != null)
                    {
                        tests.Add(line);
                        line = sr.ReadLine();
                    }
                    sr.Close();
                } 
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
        }
        public bool ExecuteTest(string test)
        {
            String[] teile=test.Split(',');
            //foreach (var str in teile) Console.Write(str+"\t"); System.Console.WriteLine();
            switch (teile[1])
            {
                case "Anruf": kommunikationsKoordinator.TestAnruf(teile[2],teile[3]) ;break;
            }
            return true;
        }
        static void Main(string[] args)
        { 
            MobilfunkSimulation simulation=new MobilfunkSimulation();
            simulation.kommunikationsKoordinator.ReadData("PhoneData.csv");
            simulation.ReadTests("Tests.csv");

            foreach (var test in simulation.tests) simulation.ExecuteTest(test);           
        }
    }
