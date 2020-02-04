# Input:
# - CSV file with merged results f2
#
# Output:
# - Convergence graphs marked area ll, lr, ul, ur
#
# Notes:
# - Use graphics.off() to terminate all output devices


# m_2 <- rbind(
#   # mapply(function(x) 54 - x * 54, results_jade$Final.Area),
#   # mapply(function(x) 54 - x * 54, results_spso$Final.Area))
#   results_jade$Final.Area,
#   results_spso$Final.Area)
# 
# par(mar=c(4.85, 4.9, 1.3, 1.3), cex=1, family="Lato")
# mp <- barplot(
#   m_2, beside=TRUE, ylim = c(0, 1),
#   ylab = "Eingesehene Fläche in %", xlab="Anzahl Agenten und Hindernisse",
#   col=c("brown1","lightblue"),
#   border=c("transparent"),
#   main = title)
# box()
# axis(1, at=seq(2, 32, by=3), labels=1:11, tick=TRUE, tck=-0.03)
# legend("bottomright", cex=0.8, horiz=TRUE,
#        legend=c("JADE", "SPSO-06"), fill=c("brown1","lightblue"), 
#        pt.bg=adjustcolor(c('red','blue','green','orange','grey')))
#' Plot graphs convergance graphs for configurations where the number
#' of agents equals the number of obstacles.
#'
#' @param data A data frame
#' @param column A string
#' @param algorithm_prefix A string
#' @param title_prefix A string
#' @param output_dir A string
#' @param levels_of_details A vector
#' @param colors A vector
#' @return Nothing.
plot_equal_team_size <- function(
  data, column, algorithm_prefix, title_prefix, output_dir, 
  levels_of_details = c(0, 0.45, 0.8, 0.95, 0.97),
  colors = c(
    "midnightblue", "orchid3", "paleturquoise3", "palevioletred3","red3", "royalblue3", 
    "salmon4", "seagreen4", "palegreen4", "slategray4", "slateblue4", "plum3")
)
{
  data_jade <- subset(
    data, Obstacles==Agents & Algorithm==paste(algorithm_prefix, "JADE", sep = ""))
  data_spso <- subset(
    data, Obstacles==Agents & Algorithm==paste(algorithm_prefix, "SPSO-2006", sep = ""))
  
  plot_convergance_graph(
    data_jade, column, algorithm_prefix, paste(title_prefix, "-", "JADE"), output_dir, levels_of_details, colors)
  plot_convergance_graph(
    data_spso, column, algorithm_prefix, paste(title_prefix, "-", "SPSO"), output_dir, levels_of_details, colors)
}

#' Plots a convergance graphs for a dataset.
#'
#' @param data_set A data frame
#' @param column A string
#' @param algorithm_prefix A string
#' @param title_prefix A string
#' @param output_dir A string
#' @param levels_of_details A vector
#' @param colors A vector
#' @return Nothing.
plot_convergance_graph <- function(
  data, column, algorithm_prefix, title_prefix, output_dir, 
  levels_of_details = c(0, 0.45, 0.8, 0.95, 0.97),
  colors = c(
    "midnightblue", "orchid3", "paleturquoise3", "palevioletred3","red3", "royalblue3", 
    "salmon4", "seagreen4", "palegreen4", "slategray4", "slateblue4", "plum3"))
{
  results_jade <- subset(
    data, Obstacles==Agents & Algorithm==paste(algorithm_prefix, "JADE", sep = ""))
  results_spso <- subset(
    data, Obstacles==Agents & Algorithm==paste(algorithm_prefix, "SPSO-2006", sep = ""))
  
  area_changes_jade <- lapply(
    strsplit(results_jade[column][[1]], ";"),
    function(y) sapply(y, function(x) as.numeric(x)))
  area_changes_spso <- lapply(
    strsplit(results_spso[column][[1]], ";"),
    function(y) sapply(y, function(x) as.numeric(x)))
  
  print(paste("Found", length(area_changes_jade), "datasets for JADE"))
  print(paste("Found", length(area_changes_spso), "datasets for SPSO"))
  
  if (length(area_changes_jade) == 0 && length(area_changes_spso) == 0)
  {
    print("Error: No datasets found")
    return()
  }
  
  cex = 1.25
  
  # Create a list of combinations with tuples defining 
  # the level of detail, the used dataset and the algorithm 
  # used for a  plot
  combinations <- list()
  i <- 1
  for (level_of_detail in levels_of_details)
  {
    combinations[[i]] <- list(level_of_detail, area_changes_spso, "SPSO")
    combinations[[i + 1]] <- list(level_of_detail, area_changes_jade, "JADE")
    i <- i + 2
  }
  
  par(mar = c(4.5, 4.5, 4, 2), cex=cex, family="Lato")
  
  for (combination in combinations)
  {
    # -- Define properties
    
    detail = combination[[1]]
    changes = combination[[2]]
    algorithm = combination[[3]]
    output_dir = ifelse(endsWith(output_dir, "/"),
                        output_dir,
                        paste(output_dir, "/", sep = ""))
    # color = ifelse(algorithm == "JADE", "brown1", "lightblue")
    output_file = paste(output_dir, "convergance-graph-", algorithm_prefix, "-",
                     floor(detail * 100), "-", tolower(algorithm), ".png", sep = "")
    output_file = gsub("--", "-", gsub(" ", "", output_file))
    title = paste(title_prefix, ", ", algorithm, ", ", floor(detail * 100), "% - 100%", sep="")
    
    # -- Activate plotting to JPEG file
    
    dir.create(file.path(output_dir), showWarnings = FALSE)
    png(filename = output_file, width = 1200, height = 1200,
         units = "px", pointsize = 10, res = "210", type = c("cairo"))
    
    # -- Plot the graph background
    
    plot(c(), ylim = c(detail, 1), xlim = c(0, 600), yaxt="n",
      xlab = "Iteration", ylab = "Gesehene Fläche in %",
      main = title, cex.lab=cex)
    grid(ny = 7)
    ylabels <- floor(seq(detail * 100, 100, (100 - detail * 100) / 6))
    yticks <- seq(detail, 1, (1 - detail) / 6)
    axis(2, at = yticks, labels = ylabels, cex.lab=cex)
    
    # -- Prepare colors for lines
    
    plot_colors = colors
    while (length(plot_colors) < length(changes))
    {
      plot_colors = append(plot_colors, colors[[length(colors)]])
    }
    
    # -- Plot lines
    
    curve_numbers = 1:length(changes)
    for (i in curve_numbers)
    {
      lines(changes[[i]], type="l", col = plot_colors[[i]], lty=1, pch="*")
      # y = changes[[i]][601][[1]]
      # agents = results_jade$Agents[[i]]
      # text(620, y, agents, cex = 1, col = "black")
    }
    
    # -- Print legend
    
    # Todo: Scale legend to fit
    legend("bottomright", cex=cex,
      legend=curve_numbers, fill=plot_colors, ncol = 2, 
      pt.bg=adjustcolor(c('red','blue','green','orange','grey')))
    
    # -- Deactivate the plot output device
    
    dev.off()
  }
  print(paste("Generated:", length(combination), "plots"))
}

# -- Plot f1

data_f1 <- read.csv("results-f1.csv", colClasses=c(
  "Algorithm"="character", "Total.Area.Changes"="character",
  "Marked.Area.Changes"="character"))

plot_convergance_graph(data_f1, "Total.Area.Changes", " ", "EF Gleiche Teamgröße", "graphs-f1/equal-team-size")
# plot_convergance_graph(data_f1, "Total.Area.Changes", " ", "EF Anwachsende Teamgröße", "graphs-f1/increasing-team-size")

# -- Plot f2

data_f2 <- read.csv("results-f2.csv", colClasses=c(
  "Algorithm"="character", "Total.Area.Changes"="character",
  "Marked.Area.Changes"="character"))

plot_convergance_graph(data_f2, "Total.Area.Changes", " ma-ll-", "EF Gesamtes Feld", "graphs-f2/total-ll")
plot_convergance_graph(data_f2, "Total.Area.Changes", " ma-lr-", "EF Gesamtes Feld", "graphs-f2/total-lr")
plot_convergance_graph(data_f2, "Total.Area.Changes", " ma-ul-", "EF Gesamtes Feld", "graphs-f2/total-ul")
plot_convergance_graph(data_f2, "Total.Area.Changes", " ma-ur-", "EF Gesamtes Feld", "graphs-f2/total-ur")

plot_convergance_graph(data_f2, "Marked.Area.Changes", " ma-ll-", "EF Markierter Bereich", "graphs-f2/ma-ll")
plot_convergance_graph(data_f2, "Marked.Area.Changes", " ma-lr-", "EF Markierter Bereich", "graphs-f2/ma-lr")
plot_convergance_graph(data_f2, "Marked.Area.Changes", " ma-ul-", "EF Markierter Bereich", "graphs-f2/ma-ul")
plot_convergance_graph(data_f2, "Marked.Area.Changes", " ma-ur-", "EF Markierter Bereich", "graphs-f2/ma-ur")

# -- Plot f3

data_f3 <- read.csv("results-f3.csv", sep = ",", colClasses=c(
  "Algorithm"="character", "Total.Area.Changes"="character",
  "Marked.Area.Changes"="character"))

plot_convergance_graph(data_f3, "Total.Area.Changes", " d-1-", "EF Distanz 1", "graphs-f3/d1")
plot_convergance_graph(data_f3, "Total.Area.Changes", " d-2-", "EF Distanz 2", "graphs-f3/d2")
plot_convergance_graph(data_f3, "Total.Area.Changes", " d-3-", "EF Distanz 3", "graphs-f3/d3")
plot_convergance_graph(data_f3, "Total.Area.Changes", " d-4-", "EF Distanz 4", "graphs-f3/d4")
plot_convergance_graph(data_f3, "Total.Area.Changes", " d-5-", "EF Distanz 5", "graphs-f3/d5")
