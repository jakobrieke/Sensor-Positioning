data <- read.csv(
  "max-area-results.csv", 
  colClasses=c("Algorithm"="character", "Changes"="character"))
data <- data[order(data$Agents, data$Obstacles),]
data_ma <- read.csv("marked-area-results.csv", colClasses=c("Algorithm"="character"))

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
changes_jade <- lapply(strsplit(results_jade$Changes, ";"), 
                  function(y) sapply(y, function(x) as.numeric(x)))
changes_spso <- lapply(strsplit(results_spso$Changes, ";"), 
                       function(y) sapply(y, function(x) as.numeric(x)))

plot(changes_spso[[1]], type = "l", col = "lightblue", 
     ylim = c(0, 30),
     xlab = "Iteration", ylab = "Ungesehene Fläche", 
     main = "")
for (i in 1:length(changes_jade)) {
  lines(changes_jade[[i]], type="l", col = "brown1")
}
for (i in 2:length(changes_spso)) {
  lines(changes_spso[[i]], type="l", col = "lightblue")
}
legend(x=600, y=30, c("JADE", "SPSO-06"), cex=.8, 
       col=c("brown1", "lightblue"), lwd=c(1, 1), xjust=1)

# -- Plot convergance graph: 1x1, 2x2, ..., 11x11 (agents times obstacles)
changes_jade_2 <- lapply(strsplit(results_jade_2$Changes, ";"), 
                       function(y) sapply(y, function(x) as.numeric(x)))
changes_spso_2 <- lapply(strsplit(results_spso_2$Changes, ";"), 
                       function(y) sapply(y, function(x) as.numeric(x)))

plot(changes_spso_2[[1]], type = "l", col = "lightblue", 
     ylim = c(0, 25),
     xlab = "Iteration", ylab = "Ungesehene Fläche", 
     main = "")
for (i in 1:length(changes_jade_2)) {
  lines(changes_jade_2[[i]], type="l", col = "brown1")
}
for (i in 2:length(changes_spso_2)) {
  lines(changes_spso_2[[i]], type="l", col = "lightblue")
}
legend(x=600, y=25, c("JADE", "SPSO-06"), cex=.8, 
       col=c("brown1", "lightblue"), lwd=c(1, 1), xjust=1)
