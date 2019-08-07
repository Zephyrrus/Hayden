﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Nito.AsyncEx;

namespace Hayden
{
	public class PoolObject<T> : IDisposable
	{
		public T Object { get; }

		private Action<T> ReturnAction { get; }

		public PoolObject(T o, Action<T> returnAction)
		{
			Object = o;
			ReturnAction = returnAction;
		}

		public void Dispose()
		{
			ReturnAction(Object);
		}

		public static implicit operator T(PoolObject<T> poolObject)
			=> poolObject.Object;
	}

	public class SqlConnectionPool : IDisposable
	{
		public AsyncCollection<MySqlConnection> Connections { get; }

		protected string ConnectionString { get; }

		protected int PoolSize { get; }

		public SqlConnectionPool(string connectionString, int poolSize)
		{
			PoolSize = poolSize;
			ConnectionString = connectionString;

			Connections = new AsyncCollection<MySqlConnection>(poolSize);

			for (int i = 0; i < poolSize; i++)
			{
				var connection = new MySqlConnection(connectionString);

				connection.Open();

				Connections.Add(connection);
			}
		}

		public async Task ForEachConnection(Func<MySqlConnection, Task> taskFunc)
		{
			List<MySqlConnection> connections = new List<MySqlConnection>();

			for (int i = 0; i < PoolSize; i++)
			{
				var currentConnection = await Connections.TakeAsync();

				await taskFunc(currentConnection);

				connections.Add(currentConnection);
			}

			foreach (var connection in connections)
				Connections.Add(connection);
		}

		public async Task<PoolObject<MySqlConnection>> RentConnectionAsync()
		{
			return new PoolObject<MySqlConnection>(await Connections.TakeAsync(), obj =>
			{
				if (obj.State != ConnectionState.Open)
				{
					Program.Log("Reviving SQL connection");
					obj.Open();
				}

				Connections.Add(obj);
			});
		}

		public PoolObject<MySqlConnection> RentConnection()
		{
			return new PoolObject<MySqlConnection>(Connections.Take(), obj =>
			{
				if (obj.State != ConnectionState.Open)
					throw new Exception("MySqlConnection state has broken");

				Connections.Add(obj);
			});
		}

		private void ReleaseUnmanagedResources()
		{
			// TODO release unmanaged resources here
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~SqlConnectionPool() {
			ReleaseUnmanagedResources();
		}
	}
}