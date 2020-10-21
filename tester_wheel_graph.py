import networkx as nx
import argparse
import matplotlib.pyplot as plt

def write_to_csv(G,path):
    df=nx.to_pandas_edgelist(G)
    df.to_csv(path)


#n = 6
parser= argparse.ArgumentParser(8)
parser.add_argument('--n', type=int)
parser.add_argument('--k', type= int)
args=parser.parse_args()
n=args.n
k=args.k
l=int(n/k)
G = nx.relaxed_caveman_graph(l,k,0)
write_to_csv(G, "C:/Users/Anni/Documents/Uni/Computer Science/Proj/CSVs and text files/FacebookUK/small_world_graph.csv")
#nx.draw(G,with_labels=False,pos=nx.spectral_layout(G),
 #      node_size=10,linewidths=0.1,width=0.01,node_color=range(1000),
  #    cmap=plt.cm.Blues)

#plt.savefig("C:/Users/Anni/Documents/Uni/Computer "
#            "Science/Proj/Network Diagrams/MostRecentNxDrawing.png")
			
print("Done with n="+str(n))
