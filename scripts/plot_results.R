data <- read.csv("max-area-results.csv", colClasses=c("Algorithm"="character"))
data <- data[order(data$Agents, data$Obstacles),]

# -- Plot area graph: [1, 2, ..., 11] x 11 (agents times obstacles)
results_jade <- subset(data, Obstacles==11 & Algorithm==" JADE")
results_spso <- subset(data, Obstacles==11 & Algorithm==" SPSO-2006")
m <- rbind(x_11_jade$Final.Area, x_11_spso$Final.Area)

par(mar=c(4.85, 4.9, 1.3, 1.3), cex=0.8, family="Lato")
mp <- barplot(
  m, beside=TRUE, ylim = c(0, max(m) + 1),
  main="", ylab = "Ungesehene Fläche", xlab="Anzahl Agenten",
  col=c("brown1","lightblue"), legend=c("JADE", "SPSO-06"),
  border=c("transparent"))
box()
axis(1, at=seq(2, 32, by=3), labels=1:11, tick=TRUE, tck=-0.03)

# -- Plot area graph: 1x1, 2x2, ..., 11x11 (agents times obstacles)
# get rows from data where num_agents x num_obstacles and numa = numo
results_jade_2 <- subset(data, Obstacles==Agents & Algorithm==" JADE")
results_spso_2 <- subset(data, Obstacles==Agents & Algorithm==" SPSO-2006")
m_2 <- rbind(results_jade_2$Final.Area, results_spso_2$Final.Area)

par(mar=c(4.85, 4.9, 1.3, 1.3), cex=0.8, family="Lato")
mp <- barplot(
  m_2, beside=TRUE, ylim = c(0, max(m_2) + 1),
  main="", ylab = "Ungesehene Fläche", xlab="Anzahl Agenten und Hindernisse",
  col=c("brown1","lightblue"), legend=c("JADE", "SPSO-06"),
  border=c("transparent"))
box()
axis(1, at=seq(2, 32, by=3), labels=1:11, tick=TRUE, tck=-0.03)

# -- Plot convergance graph: [1, 2, ..., 11] x 11 (agents times obstacles)
library("XML")
library("methods")

parse_changes <- function(x)
{
  str_tuple <- strsplit(gsub("\\[|\\]| ", "", x), ",")[[1]]
  cbind(as.integer(str_tuple[[1]]), as.numeric(str_tuple[[2]]))
}

doc <- xmlParse(file = "../results/1-agent-1-obstacle-JADE/run-2019-10-06T06-27-44Z138.log")
r1 <- xmlToList(xpathApply(doc, "//iteration[@i='600']/changes/text()")[[1]])
r2 <- as.vector(strsplit(r1,"],"), mode="list")[[1]]
changes <- sapply(r2, parse_changes)

# -- Plot convergance graph: 1x1, 2x2, ..., 11x11 (agents times obstacles)