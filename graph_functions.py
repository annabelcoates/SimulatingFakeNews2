import networkx as nx
import matplotlib.pyplot as plt
import pandas as pd

def draw_FR_graph(path):
    G=nx.from_pandas_edgelist(pd.read_csv(path))
    nx.draw(G, with_labels=True, node_size=150, node_color="skyblue",
            pos=nx.fruchterman_reingold_layout(G))
    plt.title("fruchterman_reingold")
    plt.show()

def write_to_csv(G,path):
    df=nx.to_pandas_edgelist(G)
    df.to_csv(path)
