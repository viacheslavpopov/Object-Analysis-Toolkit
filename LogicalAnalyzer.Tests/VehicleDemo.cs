﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoreLinq;

namespace Microsoft.CST.LogicalAnalyzer.Tests
{
    [TestClass]
    public class VehicleDemo
    {
        class Vehicle
        {
            public int Weight { get; set; }
            public int Axles { get; set; }
            public int Occupants { get; set; }
        }

        int GetCost(Vehicle vehicle, Analyzer analyzer, IEnumerable<Rule> rules)
        {
            return ((VehicleRule)analyzer.Analyze(rules, vehicle).MaxBy(x => x.Severity).First()).Cost;
        }
        public class VehicleRule : Rule
        {
            public int Cost;
            public VehicleRule(string name) : base(name) { }
        }

        [TestMethod]
        public void TestVehicleDemo()
        {
            var truck = new Vehicle()
            {
                Weight = 20000,
                Axles = 5,
                Occupants = 1
            };

            var car = new Vehicle()
            {
                Weight = 3000,
                Axles = 2,
                Occupants = 1
            };

            var carpool = new Vehicle()
            {
                Weight = 3000,
                Axles = 2,
                Occupants = 3
            };

            var motorcycle = new Vehicle()
            {
                Weight = 1000,
                Axles = 2,
                Occupants = 1
            };

            var rules = new VehicleRule[] {
                new VehicleRule("Overweight or long")
                {
                    Cost = 10,
                    Severity = 3,
                    Expression = "Weight OR Axles",
                    Target = "Vehicle",
                    Clauses = new List<Clause>()
                    {
                        new Clause("Weight", OPERATION.GT)
                        {
                            Label = "Weight",
                            Data = new List<string>()
                            {
                                "4000"
                            }
                        },
                        new Clause("Axles", OPERATION.GT)
                        {
                            Label = "Axles",
                            Data = new List<string>()
                            {
                                "2"
                            }
                        }
                    }
                },
                new VehicleRule("Normal Car"){
                    Cost = 3,
                    Severity = 1,
                    Target = "Vehicle",
                    Clauses = new List<Clause>()
                    {
                        new Clause("Weight", OPERATION.GT)
                        {
                            Data = new List<string>()
                            {
                                "1000"
                            }
                        }
                    }
                },
                new VehicleRule("Carpool Car"){
                    Cost = 2,
                    Severity = 2,
                    Target = "Vehicle",
                    Expression = "WeightGT1000 AND WeightLT4000 AND OccupantsGT2",
                    Clauses = new List<Clause>()
                    {
                        new Clause("Weight", OPERATION.GT)
                        {
                            Label = "WeightGT1000",
                            Data = new List<string>()
                            {
                                "1000"
                            }
                        },
                        new Clause("Weight", OPERATION.LT)
                        {
                            Label = "WeightLT4000",
                            Data = new List<string>()
                            {
                                "4000"
                            }
                        },
                        new Clause("Occupants", OPERATION.GT)
                        {
                            Label = "OccupantsGT2",
                            Data = new List<string>()
                            {
                                "2"
                            }
                        },
                    }
                },
                new VehicleRule("Motorcycle"){
                    Cost = 1,
                    Severity = 0,
                    Target = "Vehicle",
                    Clauses = new List<Clause>()
                    {
                        new Clause("Weight", OPERATION.LT)
                        {
                            Data = new List<string>()
                            {
                                "1001"
                            }
                        }
                    }
                }
            };
            var analyzer = new Analyzer();

            Assert.IsTrue(GetCost(truck, analyzer, rules) == 10);// 10
            Assert.IsTrue(GetCost(car, analyzer, rules) == 3); // 3
            Assert.IsTrue(GetCost(carpool, analyzer, rules) == 2); // 2 
            Assert.IsTrue(GetCost(motorcycle, analyzer, rules) == 1); // 1
        }
    }
}
