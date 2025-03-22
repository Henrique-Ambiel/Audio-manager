# 📢 Sistema de Gerenciamento de Áudio para Jogos
Este repositório contém um projeto desenvolvido como parte da matéria de Estudos Avançados do curso de Jogos Digitais da PUC Campinas. O objetivo principal é aprender a implementar e gerenciar a reprodução de áudio dentro de jogos utilizando Unity.

## 🕹️ Sobre o Projeto
O projeto implementa um sistema de gerenciamento de áudio, incluindo música de fundo e efeitos sonoros (SFX), usando a Unity. O sistema oferece funcionalidades como:

- Reproduzir música em loop ou uma vez.

- Controlar o volume e pitch de músicas e efeitos sonoros.

- Implementar fade in/out de música para transições suaves.

- Utilizar um pool de fontes de áudio para otimizar a reprodução de efeitos sonoros e evitar sobrecarga de performance.

## 🎵 Funcionalidades

- Música de Fundo: Toca e controla a música do jogo com suporte para fade in/out, loop e controle de volume.

- Efeitos Sonoros (SFX): Reproduz efeitos sonoros aleatórios ou específicos com suporte a volume e pitch dinâmicos, com a possibilidade de reprodução em loop.

- Pooling de Fontes de Áudio: Otimiza o gerenciamento de múltiplas fontes de áudio utilizando um pool de objetos AudioSource, evitando a criação constante de novas instâncias de fontes de áudio.

- Controles de Pausa/Parada: Permite pausar, retomar e parar os efeitos sonoros e a música.

## 🚀 Tecnologias Utilizadas

- Linguagem: C#

- Plataforma: Unity

## ⚙️ Como Funciona
- 1️⃣ O AudioManager gerencia duas fontes principais de áudio: música e efeitos sonoros (SFX).
- 2️⃣ A música pode ser tocada com controle total de volume, pitch e loop, incluindo transições suaves (fade in/out).
- 3️⃣ O sistema de efeitos sonoros usa um pool de fontes de áudio para melhorar a performance ao tocar múltiplos sons simultaneamente.
- 4️⃣ O código utiliza o padrão Singleton para garantir uma única instância do GameManager, que gerencia a inicialização e controle de áudio.

## 📌 Observações Importantes
- 🔹 Para que o código funcione corretamente, é importante configurar a referência do AudioMixer para garantir que os efeitos sonoros e a música sejam mixados corretamente na Unity.
- 🔹 O código foi desenvolvido para fins educacionais, servindo como base para aprender o gerenciamento de áudio em jogos.

🎮 Este projeto foi desenvolvido para aprimorar conhecimentos sobre o controle e otimização de áudio dentro de jogos digitais.
