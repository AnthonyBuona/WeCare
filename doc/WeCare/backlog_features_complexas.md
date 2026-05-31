# 🎯 Backlog de Features Complexas e de Alto Impacto: WeCare

Este documento define e especifica quatro novas soluções de engenharia e negócios de altíssimo valor técnico e clínico para o **WeCare**. Cada feature foi desenhada sob a ótica de um Product Owner elite para resolver as dores reais da Dra. Mariana (Clínica) e do Carlos (Responsável), aproveitando as capacidades robustas do ecossistema `.NET 9`, `ABP Framework` e `EF Core`.

---

## 🚀 1. Motor de IA Clínica para Modelagem e Predição Comportamental (WeCare AI-Insights)

### 📊 Roteiro e Proposta de Valor
- **Público Alvo:** Dra. Mariana (Gestora/Supervisora) e Carlos (Pai do Lucas).
- **Proposta de Valor:** Terapeutas de TEA registram diariamente dezenas de comportamentos-alvo (autoagressões, estereotipias, ecolalia). No entanto, analisar padrões, tendências e prever regressões clínicas em meio a planilhas é humanamente inviável no dia a dia.
- **A Solução:** Um motor inteligente baseado em aprendizado de máquina (via ML.NET ou integrações Azure OpenAI) que consome o banco histórico de `TreinosRealizados` (PerformedTraining) e logs de comportamento. Ele gera:
  1. **Predições de Regressão:** Alertas preditivos indicando probabilidade de retrocesso em objetivos como "Coordenação Fina".
  2. **Identificação de Gatilhos:** Identificação automática se fatores externos (ex: mudança de terapeuta, falha em treino doméstico registrado pelo pai) correlacionam-se com picos de comportamento inadequado.
- **Impacto no Negócio:** Retenção massiva de clientes através da comprovação científica do progresso do paciente e posicionamento premium que justifica mensalidades do tier *Scale*.

### 💻 Complexidade Técnica & Backend (.NET 9 / ABP / EF Core)
- **Desafios Arquiteturais:**
  - **Isolamento de Base (Multi-tenancy):** A IA não pode misturar dados de prontuários sensíveis de diferentes clínicas (LGPD). O treinamento de modelos locais ou envios de embeddings de dados clínicos para LLMs devem respeitar estritamente o `TenantId` ativo.
  - **Processamento Assíncrono e Background Jobs:** O cálculo e treinamento do modelo de comportamento do paciente Lucas consome memória e CPU. Deve ser delegado a workers rodando em background (via **Hangfire** ou **ABP Background Jobs**) durante a madrugada.
- **Estrutura de Entidades e Regras de Domínio:**
  - `BehavioralRecord` (Entidade Agregada): Registra a intensidade, duração e frequência de comportamentos inadequados em sessões.
  - `AISessionInsight` (Entidade Multi-tenant): Armazena as análises de IA geradas a cada ciclo.
  - `ClinicalRegressionAlert` (Entidade com regras de ativação): Alertas com criticidade baseados em limites estatísticos calculados (ex: desvio padrão de sucesso de treinos diários cai > 1.5).
- **EF Core & Banco de Dados:**
  - Índices compostos de banco de dados (`PatientId`, `DateRecorded`, `BehaviorType`) para buscas de alta performance sobre milhares de registros diários para alimentar o pipeline de análise.

### 🎨 Requisitos de Frontend e UX (Glassmorphism & Pastel Clear)
- **Dashboard de IA do Terapeuta:**
  - Visualização baseada em cartões glassmórficos com fundo desfocado e bordas finas semi-transparentes.
  - Gráficos de linha interativos (usando ApexCharts/Chart.js) com gradientes em cores pastéis suaves (verde pastel para progresso estável, âmbar pastel para atenção, e lilás para predição da IA).
- **Notificação Preditiva:**
  - Um widget flutuante no topo do prontuário indicando o "Clima Clínico" do paciente, com ícones translúcidos de nuvem ou sol pastel representando a estabilidade emocional e cognitiva prevista.

---

## 🤝 2. Prontuário Multidisciplinar Unificado Cross-Tenant com Consentimento LGPD Dinâmico

### 📊 Roteiro e Proposta de Valor
- **Público Alvo:** Dra. Mariana, terapeutas multidisciplinares (T.O., Fonoaudiologia, Psicologia) e Carlos.
- **Proposta de Valor:** Frequentemente, uma criança com TEA realiza Terapia Ocupacional em uma clínica A, e Psicopedagogia na clínica B. Hoje, os terapeutas trabalham totalmente às cegas, sem saber o que o colega está desenvolvendo.
- **A Solução:** Um ecossistema de compartilhamento de prontuário cross-tenant. O pai (Carlos) gera um token de acesso temporário com permissões cirúrgicas de leitura/escrita no seu portal. Um terapeuta de outro tenant (clínica diferente no WeCare) insere esse token e ganha acesso controlado à linha do tempo multidisciplinar do paciente.
- **Impacto no Negócio:** Transforma o WeCare em uma rede clínica integrada de fato, atraindo terapeutas autônomos que desejam trabalhar em sincronia com clínicas de grande porte.

### 💻 Complexidade Técnica & Backend (.NET 9 / ABP / EF Core)
- **Desafios Arquiteturais:**
  - **Desvio do Filtro Global do ABP:** Por padrão, o ABP filtra todas as queries pelo `TenantId` corrente (Multi-tenancy rígido). Para ler dados de outro tenant, a arquitetura deve utilizar o bypass do filtro do EF Core (`using (_currentTenant.Change(null))` combinado com `using (DataFilters.Disable<IMultiTenant>())`).
  - **Criptografia e Chave do Token:** O token deve expirar dinamicamente, ser revogável a qualquer instante pelo responsável e conter uma chave criptográfica simétrica (AES-256) gerada com base no consentimento do pai, descriptografando os dados médicos sensíveis (PHI) sob demanda na memória.
- **Estrutura de Entidades e Regras de Domínio:**
  - `CrossTenantAccessConsent` (Entidade): Modela o consentimento de compartilhamento.
    - Atributos: `Id`, `PatientId`, `SourceTenantId`, `TargetTenantId`, `ExpirationDate`, `GrantedPermissions` (Read/Write flags), `AuthTokenHash`.
  - `SharedAccessAuditLog` (Entidade imutável): Logs de auditoria rigorosos exigidos pela LGPD para registrar cada vez que um profissional de fora visualizou ou editou o prontuário do paciente Lucas.
- **EF Core & Banco de Dados:**
  - Mapeamento preciso de chaves estrangeiras entre diferentes schemas de banco de dados (se houver isolamento físico de banco) ou gerenciamento inteligente de transações distribuídas.

### 🎨 Requisitos de Frontend e UX (Glassmorphism & Pastel Clear)
- **Portal de Consentimento do Carlos (Mobile/Web):**
  - Tela minimalista pastel com cartões translúcidos exibindo os acessos ativos (ex: *"Dra. Camila - Terapia Ocupacional - Acesso Válido até 30/06"*).
  - Botão de ação rápida (efeito *glassmorphic glow* em vermelho pastel) para revogar instantaneamente qualquer acesso externo.
- **Painel do Terapeuta Externo:**
  - Uma linha do tempo integrada onde dados originados de outros tenants aparecem com um badge translúcido de identificação da clínica de origem (ex: *"Origem: Clínica Crescer"*), garantindo clareza total de autoria multidisciplinar.

---

## 💳 3. Módulo Integrado de Faturamento de Convênios & XML TISS/TUSS Automático

### 📊 Roteiro e Proposta de Valor
- **Público Alvo:** Dra. Mariana Costa (Gestora da Clínica).
- **Proposta de Valor:** O maior gargalo de faturamento em clínicas de TEA no Brasil é o envio manual de guias para planos de saúde no padrão XML TISS (ANS). Terapeutas perdem horas transcrevendo atendimentos diários em guias de consulta e códigos TUSS.
- **A Solução:** Geração automática e automatizada de faturamento XML TISS com base nas `Consultas` presenciais realizadas e validadas pelo WeCare. O sistema mapeia os treinos e consultas de T.O., Fonoaudiologia e Psicopedagogia aos respectivos códigos de procedimentos da TUSS, consolidando-os em lotes assinados prontos para envio.
- **Impacto no Negócio:** Redução de glosas de planos de saúde a praticamente zero. Economia de mais de 40 horas mensais da equipe administrativa da Dra. Mariana, acelerando o retorno financeiro (fluxo de caixa).

### 💻 Complexidade Técnica & Backend (.NET 9 / ABP / EF Core)
- **Desafios Arquiteturais:**
  - **Serialização XML Rigorosa:** A ANS impõe schemas XSD de altíssima complexidade (v4.01.00 ou superior). O backend deve possuir um pipeline de geração de XML dinâmico que valide estruturalmente os arquivos XML gerados contra as regras oficiais antes de gerar o arquivo final.
  - **Assinatura Digital de Lote:** Integração com certificados digitais (e-CNPJ da clínica no formato A1) para assinar criptograficamente os arquivos XML TISS gerados diretamente no servidor em .NET 9.
- **Estrutura de Entidades e Regras de Domínio:**
  - `BillingGuide` (Guia de Faturamento TISS): Entidade com regras rígidas de validação de dados obrigatórios (ex: número da carteirinha, código do prestador, senha da guia de autorização).
  - `TussProcedureMapping` (Entidade de Domínio): Mapeamento entre a Especialidade da Clínica, Tipo de Terapia e Código TUSS correspondente (ex: fonoaudiologia individual - `50000470`).
  - `BillingBatch` (Lote de Guias): Representa o arquivo XML consolidado emitido para a operadora de saúde.
- **EF Core & Banco de Dados:**
  - Transações atômicas de banco ao consolidar um lote para garantir que nenhuma consulta seja faturada em duplicidade caso dois faturistas operem simultaneamente no tenant.

### 🎨 Requisitos de Frontend e UX (Glassmorphism & Pastel Clear)
- **Centro de Faturamento da Clínica:**
  - Interface com visual estilo *dashboard financeiro limpo*. Cartões de métricas translúcidos no topo exibindo: *"Valor a Receber"*, *"Guias Pendentes"*, e *"Glosas"* com tipografia elegante e cores pastéis quentes.
  - Grid de dados (Datatable) com filtros rápidos por operadora de saúde (Amil, Unimed, Bradesco).
  - Um widget interativo estilo "Dropzone" em vidro jateado para importar arquivos de retorno das operadoras e conciliar pagamentos automaticamente.

---

## 🎮 4. Trilha Gamificada de Micro-Learning e Prática de Casa para os Responsáveis

### 📊 Roteiro e Proposta de Valor
- **Público Alvo:** Carlos (Pai) e Lucas (Criança).
- **Proposta de Valor:** A evolução terapêutica do autismo dobra de velocidade quando os pais treinam as metas em casa. No entanto, orientações tradicionais causam esgotamento mental nos pais (burnout de cuidadores) ou se perdem em conversas de WhatsApp.
- **A Solução:** Um aplicativo gamificado no portal do responsável. Em vez de jargões técnicos, Carlos interage com uma trilha visual semelhante a um "mapa de jogo" (como Duolingo ou RPGs de desenvolvimento). Cada "Quest" diária representa uma orientação prática do terapeuta (ex: *"Praticar abotoar camisa por 3 minutos"*). Carlos assiste a micro-vídeos de 1 minuto, executa a atividade com Lucas, registra o resultado com um slider de emojis simples, ganha XP, desbloqueia emblemas de conquistas familiares e retroalimenta o terapeuta em tempo real.
- **Impacto no Negócio:** Retenção e engajamento da família espetaculares. Fornece à clínica dados reais sobre a adesão familiar ao tratamento para fins de eficácia terapêutica.

### 💻 Complexidade Técnica & Backend (.NET 9 / ABP / EF Core)
- **Desafios Arquiteturais:**
  - **Mecanismo de Cálculo de Gamificação em Tempo Real:** O cálculo de sequências diárias (streaks), experiência ganha (XP), subida de nível e distribuição de insígnias deve ser desacoplado e performático, usando técnicas de cache com **Redis** para operações frequentes do usuário no app móvel.
  - **Integração de Mídia e Streaming Seguro:** O armazenamento de micro-vídeos de capacitação (desenvolvidos pelos terapeutas para os pais) no Azure Blob Storage com entrega via SAS Tokens seguros e integrados ao CDN para evitar carregamentos lentos.
- **Estrutura de Entidades e Regras de Domínio:**
  - `Quest` (Atividade de Casa Recomendada): Vinculada ao Objetivo de progresso (ex: "Coordenação Fina").
  - `QuestExecutionLog` (Entidade com regras comportamentais): Armazena a execução do pai, o emoji de engajamento do Lucas, dificuldades sentidas e notas de áudio/texto curtas.
  - `UserGamifiedProfile` (Entidade Multi-tenant): Mantém o nível, XP acumulado, conquistas e quantidade de dias consecutivos de trilha ativa.
- **EF Core & Banco de Dados:**
  - Consultas otimizadas com o EF Core para carregar dinamicamente o status de progresso das trilhas e suas conquistas relacionadas sem gerar sobrecarga de consultas desnecessárias (*N+1 query problem*).

### 🎨 Requisitos de Frontend e UX (Glassmorphism & Pastel Clear)
- **Portal Mobile de Gamificação (App do Carlos):**
  - Fundo degradê suave em tons pastéis de azul céu e roxo lavanda.
  - Uma trilha visual composta por nós circulares semi-transparentes (*glassmorphic nodes*) que se conectam por linhas tracejadas animadas.
  - Animações leves Lottie de confetes pastéis ao completar uma "Quest" diária com sucesso.
  - Visualizadores de micro-vídeos embutidos em modais jateados que flutuam sobre a tela principal com cantos arredondados generosos.
