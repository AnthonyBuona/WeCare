# 🗺️ Jornada do Cliente & Service Blueprint

Este documento mapeia o ciclo completo de interação do cliente (do gestor da clínica ao responsável pelo paciente) com o **WeCare**, detalhando as interações visíveis (frontstage), invisíveis (backstage) e os critérios de validação de hipóteses de negócio.

---

## 🧭 1. A Jornada do Cliente (Fases de Venda e Uso)

A experiência com o WeCare é dividida em 5 fases sequenciais:

1.  **Descoberta:** O gestor toma conhecimento da plataforma por canais digitais ou indicações.
2.  **Primeiro Contato / Triagem:** Demonstração interativa e testes iniciais do sistema.
3.  **Compra / Adesão:** Assinatura do contrato SaaS, concordância com termos LGPD e pagamento.
4.  **Uso / Atendimento:** Cadastro de profissionais, pacientes e início dos registros clínicos (PEIs, Evolução, Frequência).
5.  **Acompanhamento / Retorno:** Geração automática de relatórios em PDF, visualização de gráficos e suporte contínuo.

---

## 🛠️ 2. Service Blueprint (Visão Operacional do WeCare)

| Fase de Serviço | Descoberta | Contato & Triagem | Compra / Adesão | Uso / Atendimento | Acompanhamento |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Ações do Cliente** | Vê publicações no LinkedIn ou recebe indicação de colegas da área. | Acessa o site institucional e agenda uma demonstração online. | Escolhe o plano (SaaS) adequado à clínica e efetua o pagamento da implantação. | Cadastra a equipe de terapeutas, pacientes e registra evoluções diárias (PEIs). | Acessa relatórios em PDF gerados automaticamente e avalia gráficos. |
| **Interação Frontstage (Visível)** | Anúncios focados no ganho de tempo e gestão terapêutica no autismo. | Reunião ao vivo com demonstrador para validar fluxos específicos do WeCare. | Tela de checkout segura, preenchimento do formulário da clínica e termos LGPD. | Interface responsiva de prontuário, dashboards e controle de frequência. | E-mails de boas práticas de uso, portal de suporte e pesquisas NPS. |
| **Interação Backstage (Invisível)** | Campanhas automatizadas de marketing e estratégias de SEO ativas. | Registro automatizado no CRM de potenciais clientes e liberação de sandbox. | Provisionamento automático do **tenant isolado** no SQL Server via script. | Processamento em nuvem Azure, backups diários e criptografia de dados sensíveis. | Monitoramento de uso, logs de acesso e análise de engajamento do cliente. |
| **Evidências Físicas/Digitais** | Página de destino (Landing Page), redes sociais da marca. | E-mail de confirmação do agendamento e credenciais da demonstração. | Contrato de adesão digital assinado, fatura emitida e dados de acesso. | Relatórios de prontuários em PDF exportáveis em tempo real. | Relatórios mensais de economia de tempo e tickets de suporte solucionados. |

---

## 🧪 3. Matriz de Validação de Hipóteses

As premissas cruciais do WeCare foram testadas e validadas através de formulários estruturados com o público-alvo (Terapeutas e Pais).

### Bloco do Canvas: Proposta de Valor
*   **Premissa (Hipótese):** O WeCare reduz em pelo menos 30% o tempo gasto com documentação clínica por semana.
*   **Como Validar:** Entrevistar 5 a 10 terapeutas de TEA sobre o tempo gasto em burocracias e expor o protótipo.
*   **Status:** **Validado** *(75% dos terapeutas indicam que passam muito tempo com documentação, reduzindo horas de terapia).*

### Bloco do Canvas: Relacionamento & LGPD
*   **Premissa (Hipótese):** O maior receio ao adotar um software de saúde multidisciplinar é o vazamento de dados de menores de idade.
*   **Como Validar:** Formulário estruturado com foco em objeções de contratação.
*   **Status:** **Validado** *(Segurança de dados e adequação à LGPD representam o segundo maior receio de gestores de clínicas).*

---

## 📋 4. Critérios de Aceitação & Indicadores de Validação (Métricas)

### Formulário 1: Responsáveis por Crianças com TEA (Família)
1.  **Indicador de Valor:** Mais de **70%** dos pais entrevistados relatam que se sentem "por fora" do tratamento ou gostariam de ter informações detalhadas e centralizadas sobre a evolução de seus filhos.
2.  **Desejo da Solução:** Mais de **80%** classificaram como *"Muito Útil"* a disponibilização de um aplicativo ou portal dedicado para acompanhamento das evoluções terapêuticas.
3.  **Barreira de Contato:** Pelo menos **40%** expressaram preocupação de que o uso do software diminua o contato humano do terapeuta com a família, validando que a plataforma deve atuar como suporte à comunicação e não substituto do feedback humano.

### Formulário 2: Gestores de Clínicas e Terapeutas (Decisores)
1.  **Economia Burocrática:** Mais de **60%** dos profissionais e gestores confirmaram que a equipe despende tempo excessivo com burocracias clínicas e relatórios semanais.
2.  **Modelo Comercial Tiers:** Mais de **50%** dos gestores entrevistados preferem um modelo comercial de **"Mensalidade Escalável (Tiers)"** baseado em faixas de capacidade (número de profissionais/pacientes ativos) em detrimento de taxas variáveis por volume de consultas.
