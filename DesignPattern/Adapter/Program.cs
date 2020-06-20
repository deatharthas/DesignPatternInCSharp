using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adapter
{
    interface IBritishStandard
    {
        void ChargeInBritishStandard();
    }

    interface IChineseStandard
    {
        void ChargeInChineseStandard();
    }

    class ElectricPower
    {
        public void Charge(IBritishStandard britishStandard)
        {
            britishStandard.ChargeInBritishStandard();
        }
    }


    class ChineseCharger : IChineseStandard
    {
        public void ChargeInChineseStandard()
        {
            Console.WriteLine("Charge in Chinese Standard");
        }
    }
    

    class ChargerAdapter : IBritishStandard
    {
        IChineseStandard realCharger;
        public ChargerAdapter(IChineseStandard stander)
        {
            this.realCharger = stander;
        }

        public void ChargeInBritishStandard()
        {
            Console.WriteLine("charging through adapter");
            realCharger.ChargeInChineseStandard();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ElectricPower electricPower = new ElectricPower();
            ChineseCharger charger = new ChineseCharger();
            ChargerAdapter adapter = new ChargerAdapter(charger);
            electricPower.Charge(adapter);
        }
    }
}
