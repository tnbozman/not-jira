.PHONY: docker-up docker-down docker-test docker-build docker-logs \
       devcontainer-up devcontainer-open devcontainer-down devcontainer-cleanup \
       help

# Default target
.DEFAULT_GOAL := help

# ── Docker Compose (local full-stack) ────────────────────────────────

## Build all Docker images
docker-build:
	@echo "🔨 Building Docker images..."
	docker compose build

## Start all services (db, keycloak, backend, frontend)
docker-up: docker-build
	@echo "🚀 Starting all services..."
	docker compose up -d
	@echo "⏳ Waiting for services to become healthy (this can take a few minutes)..."
	@$(MAKE) --no-print-directory _wait-for-services
	@echo ""
	@echo "✅ All services are up!"
	@echo "   Frontend : http://localhost:80"
	@echo "   Backend  : http://localhost:8080"
	@echo "   Keycloak : http://localhost:8180"
	@echo "   Postgres : localhost:5432"

## Stop and remove all services
docker-down:
	@echo "🛑 Stopping all services..."
	docker compose down
	@echo "✅ All services stopped."

## Show logs for all services
docker-logs:
	docker compose logs -f

## Test that all services are healthy and responding
docker-test:
	@echo "🧪 Testing services..."
	@PASS=true; \
	echo ""; \
	echo "── Database (PostgreSQL) ──────────────────────────"; \
	if docker compose exec -T db pg_isready -U postgres > /dev/null 2>&1; then \
		echo "  ✅ PostgreSQL is ready"; \
	else \
		echo "  ❌ PostgreSQL is NOT ready"; \
		PASS=false; \
	fi; \
	echo ""; \
	echo "── Keycloak ────────────────────────────────────────"; \
	if curl -sf http://localhost:8180/realms/notjira > /dev/null 2>&1; then \
		echo "  ✅ Keycloak is healthy (realm 'notjira' accessible)"; \
	else \
		echo "  ❌ Keycloak is NOT healthy"; \
		PASS=false; \
	fi; \
	echo ""; \
	echo "── Backend (.NET API) ─────────────────────────────"; \
	if curl -sf http://localhost:8080/api/weatherforecast > /dev/null 2>&1; then \
		echo "  ✅ Backend API is responding"; \
	else \
		echo "  ❌ Backend API is NOT responding"; \
		PASS=false; \
	fi; \
	echo ""; \
	echo "── Frontend (Nginx) ────────────────────────────────"; \
	if curl -sf http://localhost:80 > /dev/null 2>&1; then \
		echo "  ✅ Frontend is responding"; \
	else \
		echo "  ❌ Frontend is NOT responding"; \
		PASS=false; \
	fi; \
	echo ""; \
	echo "── Docker Health Status ────────────────────────────"; \
	docker compose ps --format "table {{.Name}}\t{{.Status}}"; \
	echo ""; \
	if [ "$$PASS" = "true" ]; then \
		echo "🎉 All services are healthy!"; \
	else \
		echo "⚠️  Some services are not healthy. Run 'make docker-logs' to investigate."; \
		exit 1; \
	fi

# ── DevContainer ─────────────────────────────────────────────────────

## Start the devcontainer environment (db + keycloak + devcontainer)
devcontainer-up:
	@echo "🚀 Starting devcontainer services..."
	docker compose -f docker-compose.dev.yml up -d
	@echo "✅ DevContainer services started."
	@echo ""
	@echo "To open in VS Code, run:"
	@echo "  make devcontainer-open"

## Open the devcontainer in VS Code via Remote Explorer (/workspace)
devcontainer-open:
	@echo "🔗 Opening devcontainer in VS Code on /workspace..."
	@if command -v devcontainer > /dev/null 2>&1; then \
		devcontainer open "$(CURDIR)"; \
	else \
		code --folder-uri "vscode-remote://dev-container+$(shell printf '%s' '$(CURDIR)' | xxd -p -c 256)/workspace"; \
	fi
	@echo "✅ VS Code should now open the devcontainer."

## Stop devcontainer services
devcontainer-down:
	@echo "🛑 Stopping devcontainer services..."
	docker compose -f docker-compose.dev.yml down
	@echo "✅ DevContainer services stopped."

## Full devcontainer cleanup: stop services, remove volumes, prune images
devcontainer-cleanup:
	@echo "🧹 Cleaning up devcontainer environment..."
	docker compose -f docker-compose.dev.yml down -v --rmi local --remove-orphans
	@echo "🗑️  Pruning dangling images..."
	docker image prune -f --filter "label=com.docker.compose.project=not-jira" 2>/dev/null || true
	@echo "✅ DevContainer cleanup complete."

# ── Internal helpers ─────────────────────────────────────────────────

_wait-for-services:
	@echo "  Waiting for database..."
	@for i in $$(seq 1 30); do \
		if docker compose exec -T db pg_isready -U postgres > /dev/null 2>&1; then \
			echo "  ✅ Database ready"; \
			break; \
		fi; \
		if [ $$i -eq 30 ]; then echo "  ❌ Database timeout"; exit 1; fi; \
		sleep 2; \
	done
	@echo "  Waiting for Keycloak (may take up to 3 minutes on first start)..."
	@for i in $$(seq 1 90); do \
		if curl -sf http://localhost:8180/realms/notjira > /dev/null 2>&1; then \
			echo "  ✅ Keycloak ready"; \
			break; \
		fi; \
		if [ $$i -eq 90 ]; then echo "  ❌ Keycloak timeout"; exit 1; fi; \
		sleep 2; \
	done
	@echo "  Waiting for Backend..."
	@for i in $$(seq 1 60); do \
		if curl -sf http://localhost:8080/api/weatherforecast > /dev/null 2>&1; then \
			echo "  ✅ Backend ready"; \
			break; \
		fi; \
		if [ $$i -eq 60 ]; then echo "  ❌ Backend timeout"; exit 1; fi; \
		sleep 2; \
	done
	@echo "  Waiting for Frontend..."
	@for i in $$(seq 1 30); do \
		if curl -sf http://localhost:80 > /dev/null 2>&1; then \
			echo "  ✅ Frontend ready"; \
			break; \
		fi; \
		if [ $$i -eq 30 ]; then echo "  ❌ Frontend timeout"; exit 1; fi; \
		sleep 2; \
	done

# ── Help ─────────────────────────────────────────────────────────────

## Show this help
help:
	@echo ""
	@echo "Not-JIRA Makefile"
	@echo "================="
	@echo ""
	@echo "Docker Compose (local full-stack):"
	@echo "  make docker-build          Build all Docker images"
	@echo "  make docker-up             Build & start all services, wait until healthy"
	@echo "  make docker-down           Stop and remove all services"
	@echo "  make docker-test           Test all services are healthy and responding"
	@echo "  make docker-logs           Tail logs from all services"
	@echo ""
	@echo "DevContainer:"
	@echo "  make devcontainer-up       Start devcontainer services (db + keycloak)"
	@echo "  make devcontainer-open     Open devcontainer in VS Code (Remote Explorer)"
	@echo "  make devcontainer-down     Stop devcontainer services"
	@echo "  make devcontainer-cleanup  Full cleanup: stop, remove volumes & images"
	@echo ""
