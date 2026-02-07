# Consultas

1. **Objetivo:** (Ex: "Escovar os dentes"). Este é o contêiner macro.
2. **Treinos (Activities/Trainings):** Vinculados ao Objetivo. (Ex: "Segurar escova", "Levar escova à boca", "Movimento de escovação"). Estes são os *tipos* de atividades que podem ser feitas para alcançar o objetivo.
3. **Consulta/Sessão:** Um registro de um encontro em uma data específica, vinculado ao Objetivo.
4. **Treinos Realizados (PerformedTraining):** O registro detalhado do que aconteceu *naquela sessão*. Ele vai apontar para um **Treino** específico (para sabermos qual atividade foi feita) e para a **Consulta** (para sabermos quando foi feita), além de conter os detalhes como tentativas, sucesso, ajuda, etc.

Visualmente, a relação fica assim:

`[Paciente]
    |
    +---- [Objetivo: Escovar os dentes]
          |
          +---- [Treino: Segurar escova]
          |
          +---- [Treino: Levar escova à boca]
          |
          +---- [Consulta: 21/08/2025]
                |
                +---- [Treino Realizado] -> aponta para [Treino: Segurar escova]
                      |   - Tentativas: 10
                      |   - Sucesso: 8
                      |
                +---- [Treino Realizado] -> aponta para [Treino: Levar escova à boca]
                      |   - Tentativas: 5
                      |   - Sucesso: 4`

Isso nos permite, no futuro, abrir o modal de "Registrar Consulta", selecionar o objetivo "Escovar os dentes", e o sistema automaticamente populará o `select` com os treinos "Segurar escova" e "Levar escova à boca". Perfeito!