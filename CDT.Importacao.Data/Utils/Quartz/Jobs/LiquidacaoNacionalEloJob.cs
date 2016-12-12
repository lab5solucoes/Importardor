﻿using CDT.Importacao.Data.Business;
using CDT.Importacao.Data.DAL.Classes;
using CDT.Importacao.Data.Model;
using CDT.Importacao.Data.Utils.Log;
using CDT.Importacao.Data.Utils.Quartz.Schedulers;
using LAB5;
using Quartz;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDT.Importacao.Data.Utils.Quartz.Jobs
{
    public class LiquidacaoNacionalEloJob : CDTJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ArquivoDAO arquivoDAO = new ArquivoDAO();
            ArquivoBO arquivoBO = null;
            Arquivo arquivo = null;
            try
            {
                JobDataMap jobDataMap = context.JobDetail.JobDataMap;
                
                arquivo = new Arquivo();
                arquivoBO = new ArquivoBO(arquivo);
                if ((arquivo = arquivoDAO.Buscar(DateTime.Now.Date)) == null)
                {
                    int idEmissor = new EmissorDAO().Buscar("CBSS").IdEmissor;
                    arquivoBO.GerarArquivo(1, idEmissor, LAB5Utils.DataUtils.RetornaDataYYYYMMDD(DateTime.Now) + "_LQDNACELO.txt");
                }
                arquivoBO.Arquivo = arquivo;
                arquivoBO.Importar();
                Logger.Info(this.ToString(), "Liquidação Nacional Elo. Arquivo importado. ", "QuartzJob");
            }
            catch(Exception ex)
            {
                Logger.Warn(this.ToString(), "Erro ao executar importação automática do arquivo de Liquidação Nacional Elo. " + ex.Message, "QuartzJob");
                throw ex;
            }
        }

        public void Start(int idAgendamento, string cronExpression)
        {
            if (cronExpression != string.Empty)
                CDTScheduler.StartJobSchedule<LiquidacaoNacionalEloJob>(cronExpression,this.ToString()+idAgendamento.ToString(),"grp_"+this.ToString()+idAgendamento.ToString());
            else
                CDTScheduler.StartJobSchedule<LiquidacaoNacionalEloJob>(this.ToString() + idAgendamento.ToString(), "grp_" + this.ToString()+idAgendamento.ToString());
        }

    }
}