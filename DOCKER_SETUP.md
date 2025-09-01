# Docker-in-Docker Setup para TestContainers

## Configuração Implementada

Foi adicionada a feature Docker-in-Docker no `.devcontainer/devcontainer.json`:

```json
"ghcr.io/devcontainers/features/docker-in-docker:2": {
  "version": "latest",
  "enableNonRootDocker": true,
  "moby": true
}
```

## Próximos Passos

### 1. Reconstruir o DevContainer

Para ativar o Docker-in-Docker, você precisa reconstruir o container:

1. No VS Code, abra a Command Palette (`Ctrl+Shift+P`)
2. Execute: `Dev Containers: Rebuild Container`
3. Aguarde a reconstrução completa

### 2. Verificar Disponibilidade do Docker

Após a reconstrução, execute:

```bash
docker --version
docker ps
```

### 3. Executar os Testes

Com Docker disponível, os TestContainers funcionarão:

```bash
dotnet test --verbosity normal
```

## Recursos Recomendados

Segundo a documentação Microsoft, para Docker-in-Docker é recomendado:

- **CPU**: 4-8 cores
- **Memory**: 16-32GB  
- **Storage**: 32-64GB

## Portas Configuradas

- **5433**: PostgreSQL TestContainers
- **5432**: PostgreSQL Aspire (quando aplicável)

## TestContainers Configuration

O `PostgreSqlFixture` está configurado para usar:
- Image: `postgres:16-alpine`
- Database: `gestao_faturas_test`
- User: `test_user`
- Password: `test_password`
- Port mapping: 5433 → 5432

## Verificação

Após reconstruir o container, todos os 22 testes de database devem passar, garantindo que:

1. ✅ PostgreSQL real está sendo usado via TestContainers
2. ✅ Sem fallback para InMemory database
3. ✅ Nenhum falso positivo nos testes
4. ✅ Testes falham adequadamente quando Docker não está disponível