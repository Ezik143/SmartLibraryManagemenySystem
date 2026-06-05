"# SmartLibraryManagemenySystem"
EmailSender in docker:
maildev:
healthcheck:
test: ["CMD-SHELL, "Wwget -0 -http://127.0.0.1:1080/healthz || exit 1"]
interval: 1s
timeout: 5s
retries: 10
image: maildev/maildev
ports: - "1025 : 1025" - "1080: 1080"
