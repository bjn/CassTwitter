using FluentCassandra;
using FluentCassandra.Connections;

namespace WebUI.Models
{
	public class CassandraContextFactory
	{
		 public CassandraContext Get()
		 {
		 	return new CassandraContext("twitter", new Server());
		 }
	}
}