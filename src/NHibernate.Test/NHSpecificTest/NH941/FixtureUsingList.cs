using System.Collections.Generic;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH941
{
	public class FixtureUsingList : TestCaseMappingByCode
	{
		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			mapper.Class<MyClass>(rc =>
			                      {
			                      	rc.Id(x => x.Id, map => map.Generator(Generators.HighLow));
			                      	rc.List(x => x.Relateds, map =>
			                      	                         {
			                      	                         	map.Key(km => km.NotNullable(true));
			                      	                         	map.Cascade(Mapping.ByCode.Cascade.All);
																												map.Index(idxm=> idxm.Column(colmap=> colmap.NotNullable(true)));
			                      	                         }, rel => rel.OneToMany());
			                      });
			mapper.Class<Related>(rc => rc.Id(x => x.Id, map => map.Generator(Generators.HighLow)));
			HbmMapping mappings = mapper.CompileMappingForAllExplicitAddedEntities();
			return mappings;
		}

		[Test]
		public void WhenSaveOneThenShouldSaveMany()
		{
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					var one = new MyClass();
					one.Relateds = new List<Related> {new Related(), new Related()};
					session.Persist(one);
					tx.Commit();
				}
			}
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.CreateQuery("delete from Related").ExecuteUpdate();
					session.CreateQuery("delete from MyClass").ExecuteUpdate();
					tx.Commit();
				}
			}
		}
	}
}