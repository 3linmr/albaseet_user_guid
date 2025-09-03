using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Logic.Tree
{
	public static class PackageConversion
	{
		private static decimal CurrentPack { get; set; }

		public static decimal GetPacking(List<PackageTreeDto> data, int fromPackageId, int toPackageId)
		{
			if (fromPackageId == toPackageId)
			{
				return CurrentPack = 1;
			}
			else
			{
				foreach (var item in data)
				{
					if (item.PackageId == fromPackageId)
					{
						if (item.PackageId == toPackageId)
						{
							return CurrentPack = item.Packing;
						}
						else
						{
							CurrentPack = item.Packing;
							if (item.Items != null)
							{
								foreach (var row in item.Items)
								{
									if (row.PackageId == toPackageId)
									{
										return CurrentPack;
									}
									else
									{
										GetChild(row, toPackageId);
									}
								}
							}
						}
					}
					else
					{
						if (item.Items != null)
						{
							GetPacking(item.Items, fromPackageId, toPackageId);
						}
					}
				}
				return CurrentPack;
			}
		}

		private static decimal GetChild(PackageTreeDto tree, int toPackageId)
		{
			if (CurrentPack == 0)
			{
				CurrentPack = tree.Packing;
			}
			else
			{
				CurrentPack *= tree.Packing;
			}
			if (tree.Items != null)
			{
				if (tree.PackageId == toPackageId)
				{
					return CurrentPack;
				}
				else
				{
					foreach (var item in tree.Items)
					{
						GetChild(item, toPackageId);
					}
				}
			}
			return CurrentPack;
		}

		public static List<PackageTreeDto> BuildPackages(List<PackageTreeDto> packages)
		{
			var allPackages = packages;
			if (allPackages.Any())
			{
				var data = allPackages.Where(x => x.MainPackageId == 0)
					.Select(x => new PackageTreeDto
					{
						PackageId = x.PackageId,
						MainPackageId = x.MainPackageId,
						Packing = x.Packing,
						Items = GetChildren(allPackages, x.PackageId)
					}).ToList();
				return data;
			}
			return new List<PackageTreeDto>();
		}

		private static List<PackageTreeDto> GetChildren(List<PackageTreeDto> packages, int parentId)
		{
			return packages.Where(x => x.MainPackageId == parentId)
				.Select(x => new PackageTreeDto
				{
					PackageId = x.PackageId,
					MainPackageId = x.MainPackageId,
					Packing = x.Packing,
					Items = GetChildren(packages, x.PackageId)
				}).ToList();
		}

		public static List<ItemPacking>? GenerateTransitivePackings(int itemId, List<ItemBarCodeDto> itemBarCodes)
		{
			var adjacencyList = itemBarCodes.Where(x => x.FromPackageId != x.ToPackageId).GroupBy(x => x.ToPackageId).ToDictionary(x => x.Key, x => x.ToList());
			var finalPackings = new List<ItemPacking>();

			foreach (var currentRootPackage in itemBarCodes)
			{
				//depth first search algorithm
				//find packing from all packages to the current root package
				var packageIdAndPackings = new Stack<Tuple<int, decimal>>([new(currentRootPackage.FromPackageId, 1.0m)]);
				var visitedNodes = new List<int>(); //used to detect cycles in the input
				while (packageIdAndPackings.Count > 0)
				{
					var topPackageId = packageIdAndPackings.Pop();
					if (!visitedNodes.Contains(topPackageId.Item1))
					{
						visitedNodes.Add(topPackageId.Item1);
					}
					else
					{
						return null;
					}

					finalPackings.Add(new ItemPacking
					{
						ItemId = itemId,
						ToPackageId = currentRootPackage.FromPackageId,
						FromPackageId = topPackageId.Item1,
						Packing = topPackageId.Item2
					});

					var childPackages = adjacencyList.GetValueOrDefault(topPackageId.Item1, []);
					foreach (var childPackage in childPackages)
					{
						var nextPacking = childPackage.Packing * topPackageId.Item2;
						packageIdAndPackings.Push(new(childPackage.FromPackageId, nextPacking));
					}
				}
			}

			return finalPackings;
		}
	}
}