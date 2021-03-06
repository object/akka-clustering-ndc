/// > .paket\paket.exe generate-load-scripts --group Main --framework net461 --type fsx
#load @".paket/load/net461/main.group.fsx"

open System
open Akka.Cluster.Tools.Singleton
open Akkling
open Petabridge.Cmd.Host
open Petabridge.Cmd.Cluster

let configWithPort (port:int) =
    let config = Configuration.parse ("""
        akka {
          actor {
            provider = cluster
          }
          remote {
            dot-netty.tcp {
              public-hostname = "localhost"
              hostname = "localhost"
              port = """ + port.ToString() + """
            }
          }
          cluster {
            roles = ["Worker"]
            seed-nodes = [ "akka.tcp://cluster-system@localhost:5000" ]
          }
        }
        """)
    config.WithFallback(ClusterSingletonManager.DefaultConfig())

let system1 = System.create "cluster-system" (configWithPort 5000)

let cmd = PetabridgeCmd.Get(system1)
cmd.RegisterCommandPalette(ClusterCommands.Instance)
cmd.Start()

let _system2 = System.create "cluster-system" (configWithPort 5001)
