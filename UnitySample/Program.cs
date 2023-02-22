using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Injection;

namespace UnitySample
{
    class Program
    {
        static void Main(string[] args)
        {
            IUnityContainer container = new UnityContainer();

            // 同じ型に対して、複数の型情報を登録するため名前付きにする
            container.RegisterType<IVehicle, Car>(nameof(Car));
            container.RegisterType<IVehicle, Bike>(nameof(Bike));
            container.RegisterType<CarDriver>(
                new InjectionConstructor(
                    new ResolvedParameter<IVehicle>(nameof(Car))
                    )
                );
            container.RegisterType<BikeDriver>(
                new InjectionConstructor(
                    new ResolvedParameter<IVehicle>(nameof(Bike))
                )
            );

            var list = container.Registrations
                .Where(r => r.RegisteredType.Name == nameof(IVehicle));
            foreach (var item in list)
            {
                Console.WriteLine($"RegType:{item.RegisteredType.Name} Name:{item.Name}");
            }

            //var names = new string[] { nameof(Car) , nameof(Bike)};
            var names = list.Select(r => r.Name);
            foreach (var name in names)
            {
                var obj = container.Resolve<IVehicle>(name);
                obj.Run();
                Console.WriteLine($"Run {obj.GetType().Name} - {obj.Run()} mile");
            }

            var tmpCar = container.Resolve<IVehicle>(nameof(Car));
            var tmpBile = container.Resolve<IVehicle>(nameof(Bike));

            if (container.Resolve<IVehicle>(nameof(Car)) is Car)
            {
                Console.WriteLine("Return Car");
            }
            if (container.Resolve<IVehicle>(nameof(Bike)) is Bike)
            {
                Console.WriteLine("Return Bike");
            }

            var carDriver = container.Resolve<CarDriver>();
            if (carDriver is CarDriver)
            {
                carDriver.Run();
            }
            var bikeDriver = container.Resolve<BikeDriver>();
            if (bikeDriver is BikeDriver)
            {
                bikeDriver.Run();
            }
        }
    }

    public interface IVehicle
    {
        int Run();
    }

    public class Car : IVehicle
    {
        private int _miles = 0;
        public int Run()
        {
            return ++_miles;
        }
    }

    public class Bike : IVehicle
    {
        private int _miles = 0;

        public int Run()
        {
            return ++_miles;
        }
    }

    public abstract class Driver
    {
        protected IVehicle Vehicle = null;
        public void Run()
        {
            Console.WriteLine($"Run {Vehicle.GetType().Name} - {Vehicle.Run()} mile");
        }
    }

    public class CarDriver : Driver
    {
        [InjectionConstructor]
        public CarDriver(IVehicle vehicle)
        {
            this.Vehicle = vehicle;
        }
    }

    public class BikeDriver : Driver
    {
        [InjectionConstructor]
        public BikeDriver(IVehicle vehicle)
        {
            this.Vehicle = vehicle;
        }
    }
}
